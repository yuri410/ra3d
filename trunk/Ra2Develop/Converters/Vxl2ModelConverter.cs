/*
 * Copyright (C) 2008 R3D Development Team
 * 
 * R3D is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * R3D is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with R3D.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Ra2Develop.Designers;
using Ra2Develop.Editors.EditableObjects;
using R3D;
using R3D.Core;
using R3D.GraphicsEngine;
using R3D.GraphicsEngine.Animating;
using R3D.IO;
using R3D.MathLib;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;
using R3D.Base;


namespace Ra2Develop.Converters
{
    public enum Vxl2MdlMode
    {
        Ra2,
        Ts
    }

    public unsafe class Vxl2ModelConverter : ConverterBase
    {
        const string CsfKey = "GUI:Vxl2Mesh";

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct Vector3i
        {
            public short X;
            public short Y;
            public short Z;

            public Vector3i(short x, short y, short z)
            {
                X = x;
                Y = y;
                Z = z;
            }
            public Vector3i(int x, int y, int z)
            {
                X = (short)x;
                Y = (short)y;
                Z = (short)z;
            }


            public static bool operator ==(Vector3i a, Vector3i b)
            {
                return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
            }
            public static bool operator !=(Vector3i a, Vector3i b)
            {
                return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
            }
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
            public override int GetHashCode()
            {
                int z = Z;
                return (int)((X << 20) | (Y << 10) | z);
                //return (int)res;
            }
        }
        struct AdjFaceInfo
        {
            /// <summary>
            /// releative position of a adj face's voxel
            /// </summary>
            public Vector3i vxlPos;
            public VxlFlag face;
        }
        class VxlFace
        {
            public Vector3i[] vtx = new Vector3i[4];

            public static bool operator ==(VxlFace a, VxlFace b)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (a.vtx[i] != b.vtx[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            public static bool operator !=(VxlFace a, VxlFace b)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (a.vtx[i] != b.vtx[i])
                    {
                        return true;
                    }
                }
                return false;
            }
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
        class VxlQuad
        {
            public Queue<VxlQuad>[] adj;

            public byte normal;

            public byte color;

            public short TexX;
            public short TexY;

            public int Index
            {
                get;
                private set;
            }

            public VxlQuad(int index)
            {
                Index = index;
                adj = new Queue<VxlQuad>[4];
                for (int i = 0; i < 4; i++) 
                {
                    adj[i] = new Queue<VxlQuad>(3);
                }
            }
            public override int GetHashCode()
            {
                return ((int)(TexX << 16)) | ((ushort)TexY);
            }
        }


        [Flags()]
        enum VxlFlag : byte
        {
            None = 0,
            Passed = 1,
            FacePosX = 1 << 1,
            FacePosY = 1 << 2,
            FacePosZ = 1 << 3,
            FaceNegX = 1 << 4,
            FaceNegY = 1 << 5,
            FaceNegZ = 1 << 6,

            TexPassed = 1 << 7
        }

        #region Normal Table
        static Vector3[] ra2Normals = new Vector3[244]
        { 
            new Vector3 { X = 0.526578009128571f,    Y = -0.359620988368988f,   Z = -0.770317018032074f },
            new Vector3 { X = 0.150481998920441f,    Y = 0.43598398566246f,     Z = 0.887283980846405f },
            new Vector3 { X = 0.414195001125336f,    Y = 0.738255023956299f,    Z = -0.532374024391174f },
            new Vector3 { X = 0.0751520022749901f,   Y = 0.916248977184296f,    Z = -0.393498003482819f },
            new Vector3 { X = -0.316148996353149f,   Y = 0.930736005306244f,    Z = -0.183792993426323f },
            new Vector3 { X = -0.773819029331207f,   Y = 0.623333990573883f,    Z = -0.112510003149509f },
            new Vector3 { X = -0.900842010974884f,   Y = 0.428537011146545f,    Z = -0.0695680007338524f },
            new Vector3 { X = -0.998942017555237f,   Y = -0.010971000418067f,   Z = 0.0446650013327599f },
            new Vector3 { X = -0.979761004447937f,   Y = -0.157670006155968f,   Z = -0.123323999345303f },
            new Vector3 { X = -0.911274015903473f,   Y = -0.362370997667313f,   Z = -0.195620000362396f },
            new Vector3 { X = -0.624068975448608f,   Y = -0.720941007137299f,   Z = -0.301301002502441f },
            new Vector3 { X = -0.310173004865646f,   Y = -0.809345006942749f,   Z = -0.498751997947693f },
            new Vector3 { X = 0.146613001823425f,    Y = -0.815819025039673f,   Z = -0.559414029121399f },
            new Vector3 { X = -0.716516017913818f,   Y = -0.694356024265289f,   Z = -0.0668879970908165f },
            new Vector3 { X = 0.503971993923187f,    Y = -0.114202000200748f,   Z = -0.856136977672577f },
            new Vector3 { X = 0.455491006374359f,    Y = 0.872627019882202f,    Z = -0.176210999488831f },
            new Vector3 { X = -0.00500999996438622f, Y = -0.114372998476028f,   Z = -0.993425011634827f },
            new Vector3 { X = -0.104675002396107f,   Y = -0.32770100235939f,    Z = -0.938965022563934f },
            new Vector3 { X = 0.560411989688873f,    Y = 0.752588987350464f,    Z = -0.345755994319916f },
            new Vector3 { X = -0.0605759993195534f,  Y = 0.821627974510193f,    Z = -0.566796004772186f },
            new Vector3 { X = -0.302341014146805f,   Y = 0.797007024288178f,    Z = -0.52284699678421f },
            new Vector3 { X = -0.671543002128601f,   Y = 0.670740008354187f,    Z = -0.314862996339798f },
            new Vector3 { X = -0.778401017189026f,   Y = -0.128356993198395f,   Z = 0.614504992961884f },
            new Vector3 { X = -0.924049973487854f,   Y = 0.278382003307343f,    Z = -0.261985003948212f },
            new Vector3 { X = -0.699773013591766f,   Y = -0.550490975379944f,   Z = -0.455278009176254f },
            new Vector3 { X = -0.568247973918915f,   Y = -0.517189025878906f,   Z = -0.640007972717285f },
            new Vector3 { X = 0.0540979988873005f,   Y = -0.932864010334015f,   Z = -0.356142997741699f },
            new Vector3 { X = 0.758382022380829f,    Y = 0.572893023490906f,    Z = -0.31088799238205f },
            new Vector3 { X = 0.00362000009045005f,  Y = 0.305025994777679f,    Z = -0.952337026596069f },
            new Vector3 { X = -0.0608499981462956f,  Y = -0.986886024475098f,   Z = -0.149510994553566f },
            new Vector3 { X = 0.635230004787445f,    Y = 0.0454780012369156f,   Z = -0.770982980728149f },
            new Vector3 { X = 0.521704971790314f,    Y = 0.241309002041817f,    Z = -0.818287014961243f },
            new Vector3 { X = 0.269403994083405f,    Y = 0.635424971580505f,    Z = -0.723640978336334f },
            new Vector3 { X = 0.0456760004162788f,   Y = 0.672753989696503f,    Z = -0.73845499753952f },
            new Vector3 { X = -0.180510997772217f,   Y = 0.674656987190247f,    Z = -0.715718984603882f },
            new Vector3 { X = -0.397130995988846f,   Y = 0.636640012264252f,    Z = -0.661041975021362f },
            new Vector3 { X = -0.552003979682922f,   Y = 0.472514986991882f,    Z = -0.687038004398346f },
            new Vector3 { X = -0.772170007228851f,   Y = 0.0830899998545647f,   Z = -0.629960000514984f },
            new Vector3 { X = -0.669818997383118f,   Y = -0.119533002376556f,   Z = -0.732840001583099f },
            new Vector3 { X = -0.540454983711243f,   Y = -0.318444013595581f,   Z = -0.77878201007843f },
            new Vector3 { X = -0.386135011911392f,   Y = -0.522789001464844f,   Z = -0.759993970394135f },
            new Vector3 { X = -0.26146599650383f,    Y = -0.688566982746124f,   Z = -0.676394999027252f },
            new Vector3 { X = -0.0194119997322559f,  Y = -0.696102976799011f,   Z = -0.717679977416992f },
            new Vector3 { X = 0.303568989038467f,    Y = -0.481844007968903f,   Z = -0.821992993354797f },
            new Vector3 { X = 0.681939005851746f,    Y = -0.195129007101059f,   Z = -0.704900026321411f },
            new Vector3 { X = -0.244889006018639f,   Y = -0.116562001407146f,   Z = -0.962518990039825f },
            new Vector3 { X = 0.800759017467499f,    Y = -0.0229790005832911f,  Z = -0.598546028137207f },
            new Vector3 { X = -0.370274990797043f,   Y = 0.0955839976668358f,   Z = -0.923991024494171f },
            new Vector3 { X = -0.330671012401581f,   Y = -0.326577991247177f,   Z = -0.885439991950989f },
            new Vector3 { X = -0.163220003247261f,   Y = -0.527579009532928f,   Z = -0.833679020404816f },
            new Vector3 { X = 0.126389995217323f,    Y = -0.313145995140076f,   Z = -0.941256999969482f },
            new Vector3 { X = 0.349548012018204f,    Y = -0.272226005792618f,   Z = -0.896498024463654f },
            new Vector3 { X = 0.239917993545532f,    Y = -0.0858250036835671f,  Z = -0.966992020606995f },
            new Vector3 { X = 0.390845000743866f,    Y = 0.0815370008349419f,   Z = -0.916837990283966f },
            new Vector3 { X = 0.2552669942379f,      Y = 0.268696993589401f,    Z = -0.928785026073456f },
            new Vector3 { X = 0.146245002746582f,    Y = 0.480437994003296f,    Z = -0.864749014377594f },
            new Vector3 { X = -0.326016008853912f,   Y = 0.478455990552902f,    Z = -0.815348982810974f },
            new Vector3 { X = -0.46968200802803f,    Y = -0.112519003450871f,   Z = -0.875635981559753f },
            new Vector3 { X = 0.818440020084381f,    Y = -0.258520007133484f,   Z = -0.513150990009308f },
            new Vector3 { X = -0.474317997694015f,   Y = 0.292237997055054f,    Z = -0.830433011054993f },
            new Vector3 { X = 0.778943002223969f,    Y = 0.395841985940933f,    Z = -0.486371010541916f },
            new Vector3 { X = 0.624094009399414f,    Y = 0.39377298951149f,     Z = -0.674870014190674f },
            new Vector3 { X = 0.740885972976685f,    Y = 0.203833997249603f,    Z = -0.639953017234802f },
            new Vector3 { X = 0.480217009782791f,    Y = 0.565768003463745f,    Z = -0.670297026634216f },
            new Vector3 { X = 0.380930006504059f,    Y = 0.424535006284714f,    Z = -0.821377992630005f },
            new Vector3 { X = -0.0934220030903816f,  Y = 0.501124024391174f,    Z = -0.860318005084991f },
            new Vector3 { X = -0.236485004425049f,   Y = 0.296198010444641f,    Z = -0.925387024879456f },
            new Vector3 { X = -0.131531000137329f,   Y = 0.0939590036869049f,   Z = -0.986849009990692f },
            new Vector3 { X = -0.823562026023865f,   Y = 0.29577699303627f,     Z = -0.484005987644196f },
            new Vector3 { X = 0.611065983772278f,    Y = -0.624303996562958f,   Z = -0.486663997173309f },
            new Vector3 { X = 0.0694959983229637f,   Y = -0.520330011844635f,   Z = -0.851132988929748f },
            new Vector3 { X = 0.226521998643875f,    Y = -0.664879024028778f,   Z = -0.711775004863739f },
            new Vector3 { X = 0.471307992935181f,    Y = -0.568903982639313f,   Z = -0.673956990242004f },
            new Vector3 { X = 0.38842499256134f,     Y = -0.74262398481369f,    Z = -0.545560002326965f },
            new Vector3 { X = 0.783675014972687f,    Y = -0.480729013681412f,   Z = -0.393384993076324f },
            new Vector3 { X = 0.962393999099731f,    Y = 0.135675996541977f,    Z = -0.235348999500275f },
            new Vector3 { X = 0.876607000827789f,    Y = 0.172033995389938f,    Z = -0.449405997991562f },
            new Vector3 { X = 0.633405029773712f,    Y = 0.589793026447296f,    Z = -0.500940978527069f },
            new Vector3 { X = 0.182275995612144f,    Y = 0.800657987594605f,    Z = -0.570720970630646f },
            new Vector3 { X = 0.177002996206284f,    Y = 0.764133989810944f,    Z = 0.620297014713287f },
            new Vector3 { X = -0.544016003608704f,   Y = 0.675514996051788f,    Z = -0.497720986604691f },
            new Vector3 { X = -0.679296970367432f,   Y = 0.286466985940933f,    Z = -0.675642013549805f },
            new Vector3 { X = -0.590390980243683f,   Y = 0.0913690030574799f,   Z = -0.801928997039795f },
            new Vector3 { X = -0.824360013008118f,   Y = -0.133123993873596f,   Z = -0.550189018249512f },
            new Vector3 { X = -0.715794026851654f,   Y = -0.334542006254196f,   Z = -0.612960994243622f },
            new Vector3 { X = 0.174285992980003f,    Y = -0.8924840092659f,     Z = 0.416049003601074f },
            new Vector3 { X = -0.0825280025601387f,  Y = -0.837122976779938f,   Z = -0.54075300693512f },
            new Vector3 { X = 0.283331006765366f,    Y = -0.88087397813797f,    Z = -0.379189014434814f },
            new Vector3 { X = 0.675134003162384f,    Y = -0.42662701010704f,    Z = -0.601817011833191f },
            new Vector3 { X = 0.843720018863678f,    Y = -0.512335002422333f,   Z = -0.16015599668026f },
            new Vector3 { X = 0.977303981781006f,    Y = -0.0985559970140457f,  Z = -0.187519997358322f },
            new Vector3 { X = 0.84629499912262f,     Y = 0.52267199754715f,     Z = -0.102946996688843f },
            new Vector3 { X = 0.677141010761261f,    Y = 0.721324980258942f,    Z = -0.145501002669334f },
            new Vector3 { X = 0.320964992046356f,    Y = 0.870891988277435f,    Z = -0.372193992137909f },
            new Vector3 { X = -0.178977996110916f,   Y = 0.911532998085022f,    Z = -0.37023600935936f },
            new Vector3 { X = -0.447169005870819f,   Y = 0.826700985431671f,    Z = -0.341473996639252f },
            new Vector3 { X = -0.703203022480011f,   Y = 0.496327996253967f,    Z = -0.50908100605011f },
            new Vector3 { X = -0.977181017398834f,   Y = 0.0635629966855049f,   Z = -0.202674001455307f },
            new Vector3 { X = -0.878170013427734f,   Y = -0.412937998771667f,   Z = 0.241455003619194f },
            new Vector3 { X = -0.835830986499786f,   Y = -0.358550012111664f,   Z = -0.415728002786636f },
            new Vector3 { X = -0.499173998832703f,   Y = -0.693432986736298f,   Z = -0.519591987133026f },
            new Vector3 { X = -0.188788995146751f,   Y = -0.923753023147583f,   Z = -0.333225011825562f },
            new Vector3 { X = 0.19225400686264f,     Y = -0.969361007213593f,   Z = -0.152896001935005f },
            new Vector3 { X = 0.515940010547638f,    Y = -0.783906996250153f,   Z = -0.345391988754272f },
            new Vector3 { X = 0.90592497587204f,     Y = -0.300951987504959f,   Z = -0.297870993614197f },
            new Vector3 { X = 0.991111993789673f,    Y = -0.127746000885963f,   Z = 0.0371069982647896f },
            new Vector3 { X = 0.995135009288788f,    Y = 0.0984240025281906f,   Z = -0.0043830000795424f },
            new Vector3 { X = 0.760123014450073f,    Y = 0.646277010440826f,    Z = 0.0673670023679733f },
            new Vector3 { X = 0.205220997333527f,    Y = 0.95958000421524f,     Z = -0.192590996623039f },
            new Vector3 { X = -0.0427500009536743f,  Y = 0.979512989521027f,    Z = -0.196790993213654f },
            new Vector3 { X = -0.438017010688782f,   Y = 0.898926973342895f,    Z = 0.00849200040102005f },
            new Vector3 { X = -0.821994006633759f,   Y = 0.480785012245178f,    Z = -0.305238991975784f },
            new Vector3 { X = -0.899917006492615f,   Y = 0.0817100033164024f,   Z = -0.428337007761002f },
            new Vector3 { X = -0.926612019538879f,   Y = -0.144618004560471f,   Z = -0.347095996141434f },
            new Vector3 { X = -0.79365998506546f,    Y = -0.557792007923126f,   Z = -0.242838993668556f },
            new Vector3 { X = -0.431349992752075f,   Y = -0.847778975963593f,   Z = -0.308557987213135f },
            new Vector3 { X = -0.00549199990928173f, Y = -0.964999973773956f,   Z = 0.262192994356155f },
            new Vector3 { X = 0.587904989719391f,    Y = -0.804026007652283f,   Z = -0.0889400020241737f },
            new Vector3 { X = 0.699492990970612f,    Y = -0.667685985565186f,   Z = -0.254765003919601f },
            new Vector3 { X = 0.889303028583527f,    Y = 0.35979500412941f,     Z = -0.282290995121002f },
            new Vector3 { X = 0.780972003936768f,    Y = 0.197036996483803f,    Z = 0.592671990394592f },
            new Vector3 { X = 0.520120978355408f,    Y = 0.506695985794067f,    Z = 0.687556982040405f },
            new Vector3 { X = 0.403894990682602f,    Y = 0.693961024284363f,    Z = 0.59605997800827f },
            new Vector3 { X = -0.154982998967171f,   Y = 0.899236023426056f,    Z = 0.409090012311935f },
            new Vector3 { X = -0.65733802318573f,    Y = 0.537168025970459f,    Z = 0.528542995452881f },
            new Vector3 { X = -0.746195018291473f,   Y = 0.334091007709503f,    Z = 0.57582700252533f },
            new Vector3 { X = -0.624952018260956f,   Y = -0.0491439998149872f,  Z = 0.77911502122879f },
            new Vector3 { X = 0.318141013383865f,    Y = -0.254714995622635f,   Z = 0.913185000419617f },
            new Vector3 { X = -0.555896997451782f,   Y = 0.405294001102447f,    Z = 0.725751996040344f },
            new Vector3 { X = -0.794434010982513f,   Y = 0.0994059965014458f,   Z = 0.599160015583038f },
            new Vector3 { X = -0.64036101102829f,    Y = -0.689463019371033f,   Z = 0.3384949862957f },
            new Vector3 { X = -0.126712992787361f,   Y = -0.734094977378845f,   Z = 0.667119979858398f },
            new Vector3 { X = 0.105457000434399f,    Y = -0.780816972255707f,   Z = 0.615795016288757f },
            new Vector3 { X = 0.407992988824844f,    Y = -0.480915993452072f,   Z = 0.776054978370666f },
            new Vector3 { X = 0.69513601064682f,     Y = -0.545120000839233f,   Z = 0.468647003173828f },
            new Vector3 { X = 0.973191022872925f,    Y = -0.00648899981752038f, Z = 0.229908004403114f },
            new Vector3 { X = 0.946893990039825f,    Y = 0.31750899553299f,     Z = -0.0507990010082722f },
            new Vector3 { X = 0.563583016395569f,    Y = 0.825612008571625f,    Z = 0.0271829999983311f },
            new Vector3 { X = 0.325773000717163f,    Y = 0.945423007011414f,    Z = 0.00694900006055832f },
            new Vector3 { X = -0.171820998191834f,   Y = 0.985096991062164f,    Z = -0.00781499966979027f },
            new Vector3 { X = -0.670440971851349f,   Y = 0.739938974380493f,    Z = 0.0547689981758594f },
            new Vector3 { X = -0.822980999946594f,   Y = 0.554961979389191f,    Z = 0.121321998536587f },
            new Vector3 { X = -0.96619302034378f,    Y = 0.117857001721859f,    Z = 0.229306995868683f },
            new Vector3 { X = -0.953769028186798f,   Y = -0.294703990221024f,   Z = 0.0589450001716614f },
            new Vector3 { X = -0.864386975765228f,   Y = -0.50272798538208f,    Z = -0.0100149996578693f },
            new Vector3 { X = -0.530609011650085f,   Y = -0.842006027698517f,   Z = -0.0973659977316856f },
            new Vector3 { X = -0.16261799633503f,    Y = -0.984075009822845f,   Z = 0.071772001683712f },
            new Vector3 { X = 0.081446997821331f,    Y = -0.996011018753052f,   Z = 0.0364390015602112f },
            new Vector3 { X = 0.745984017848968f,    Y = -0.665962994098663f,   Z = 0.000761999981477857f },
            new Vector3 { X = 0.942057013511658f,    Y = -0.329268991947174f,   Z = -0.0641060024499893f },
            new Vector3 { X = 0.939701974391937f,    Y = -0.2810899913311f,     Z = 0.19480299949646f },
            new Vector3 { X = 0.771214008331299f,    Y = 0.550670027732849f,    Z = 0.319362998008728f },
            new Vector3 { X = 0.641348004341126f,    Y = 0.730690002441406f,    Z = 0.234020993113518f },
            new Vector3 { X = 0.0806820020079613f,   Y = 0.996690988540649f,    Z = 0.00987899955362082f },
            new Vector3 { X = -0.0467250011861324f,  Y = 0.976643025875092f,    Z = 0.209725007414818f },
            new Vector3 { X = -0.531076014041901f,   Y = 0.821000993251801f,    Z = 0.209562003612518f },
            new Vector3 { X = -0.695815026760101f,   Y = 0.65599000453949f,     Z = 0.292434990406036f },
            new Vector3 { X = -0.97612202167511f,    Y = 0.21670900285244f,     Z = -0.0149130001664162f },
            new Vector3 { X = -0.961660981178284f,   Y = -0.144128993153572f,   Z = 0.233313992619514f },
            new Vector3 { X = -0.77208399772644f,    Y = -0.613646984100342f,   Z = 0.165298998355865f },
            new Vector3 { X = -0.449600011110306f,   Y = -0.836059987545013f,   Z = 0.314426004886627f },
            new Vector3 { X = -0.392699986696243f,   Y = -0.914615988731384f,   Z = 0.0962470024824142f },
            new Vector3 { X = 0.390588998794556f,    Y = -0.919470012187958f,   Z = 0.0448900014162064f },
            new Vector3 { X = 0.582529008388519f,    Y = -0.799197971820831f,   Z = 0.148127004504204f },
            new Vector3 { X = 0.866430997848511f,    Y = -0.489811986684799f,   Z = 0.0968639999628067f },
            new Vector3 { X = 0.904586970806122f,    Y = 0.11149799823761f,     Z = 0.411449998617172f },
            new Vector3 { X = 0.953536987304687f,    Y = 0.232329994440079f,    Z = 0.191806003451347f },
            new Vector3 { X = 0.497310996055603f,    Y = 0.770802974700928f,    Z = 0.398176997900009f },
            new Vector3 { X = 0.194066002964973f,    Y = 0.956319987773895f,    Z = 0.218611001968384f },
            new Vector3 { X = 0.422876000404358f,    Y = 0.882275998592377f,    Z = 0.206797003746033f },
            new Vector3 { X = -0.373796999454498f,   Y = 0.849565982818604f,    Z = 0.372173994779587f },
            new Vector3 { X = -0.534497022628784f,   Y = 0.714022994041443f,    Z = 0.452199995517731f },
            new Vector3 { X = -0.881826996803284f,   Y = 0.237159997224808f,    Z = 0.407597988843918f },
            new Vector3 { X = -0.904947996139526f,   Y = -0.0140690002590418f,  Z = 0.425289005041122f },
            new Vector3 { X = -0.751827001571655f,   Y = -0.512817025184631f,   Z = 0.414458006620407f },
            new Vector3 { X = -0.50101500749588f,    Y = -0.697916984558105f,   Z = 0.511758029460907f },
            new Vector3 { X = -0.235190004110336f,   Y = -0.925922989845276f,   Z = 0.295554995536804f },
            new Vector3 { X = 0.228982999920845f,    Y = -0.953939974308014f,   Z = 0.193819001317024f },
            new Vector3 { X = 0.734025001525879f,    Y = -0.634898006916046f,   Z = 0.241062000393867f },
            new Vector3 { X = 0.913752973079681f,    Y = -0.0632530003786087f,  Z = -0.401315987110138f },
            new Vector3 { X = 0.905735015869141f,    Y = -0.161486998200417f,   Z = 0.391874998807907f },
            new Vector3 { X = 0.858929991722107f,    Y = 0.342445999383926f,    Z = 0.380748987197876f },
            new Vector3 { X = 0.624486029148102f,    Y = 0.60758101940155f,     Z = 0.490776985883713f },
            new Vector3 { X = 0.289263993501663f,    Y = 0.857478976249695f,    Z = 0.425507992506027f },
            new Vector3 { X = 0.0699680000543594f,   Y = 0.902168989181519f,    Z = 0.425671011209488f },
            new Vector3 { X = -0.28617998957634f,    Y = 0.940699994564056f,    Z = 0.182164996862411f },
            new Vector3 { X = -0.574012994766235f,   Y = 0.805118978023529f,    Z = -0.149308994412422f },
            new Vector3 { X = 0.111258000135422f,    Y = 0.0997179970145225f,   Z = -0.988776028156281f },
            new Vector3 { X = -0.305393010377884f,   Y = -0.944227993488312f,   Z = -0.123159997165203f },
            new Vector3 { X = -0.601166009902954f,   Y = -0.78957599401474f,    Z = 0.123162999749184f },
            new Vector3 { X = -0.290645003318787f,   Y = -0.812139987945557f,   Z = 0.505918979644775f },
            new Vector3 { X = -0.064920000731945f,   Y = -0.877162992954254f,   Z = 0.475784987211227f },
            new Vector3 { X = 0.408300995826721f,    Y = -0.862215995788574f,   Z = 0.299789011478424f },
            new Vector3 { X = 0.566097021102905f,    Y = -0.725566029548645f,   Z = 0.391263991594315f },
            new Vector3 { X = 0.839363992214203f,    Y = -0.427386999130249f,   Z = 0.335869014263153f },
            new Vector3 { X = 0.818899989128113f,    Y = -0.0413050018250942f,  Z = 0.572448015213013f },
            new Vector3 { X = 0.719784021377564f,    Y = 0.414997011423111f,    Z = 0.556496977806091f },
            new Vector3 { X = 0.881744027137756f,    Y = 0.450269997119904f,    Z = 0.140659004449844f },
            new Vector3 { X = 0.40182301402092f,     Y = -0.898220002651215f,   Z = -0.178151994943619f },
            new Vector3 { X = -0.0540199987590313f,  Y = 0.791343986988068f,    Z = 0.608980000019074f },
            new Vector3 { X = -0.293774008750916f,   Y = 0.763993978500366f,    Z = 0.574464976787567f },
            new Vector3 { X = -0.450798004865646f,   Y = 0.610346972942352f,    Z = 0.651350975036621f },
            new Vector3 { X = -0.638221025466919f,   Y = 0.186693996191025f,    Z = 0.746873021125793f },
            new Vector3 { X = -0.872870028018951f,   Y = -0.257126986980438f,   Z = 0.414707988500595f },
            new Vector3 { X = -0.587257027626038f,   Y = -0.521709978580475f,   Z = 0.618827998638153f },
            new Vector3 { X = -0.353657990694046f,   Y = -0.641973972320557f,   Z = 0.680290997028351f },
            new Vector3 { X = 0.0416489988565445f,   Y = -0.611272990703583f,   Z = 0.79032301902771f },
            new Vector3 { X = 0.348342001438141f,    Y = -0.779182970523834f,   Z = 0.521086990833282f },
            new Vector3 { X = 0.499166995286942f,    Y = -0.622440993785858f,   Z = 0.602825999259949f },
            new Vector3 { X = 0.790018975734711f,    Y = -0.3038310110569f,     Z = 0.53250002861023f },
            new Vector3 { X = 0.660117983818054f,    Y = 0.0607330016791821f,   Z = 0.748701989650726f },
            new Vector3 { X = 0.604920983314514f,    Y = 0.29416099190712f,     Z = 0.739960014820099f },
            new Vector3 { X = 0.38569700717926f,     Y = 0.379346013069153f,    Z = 0.841032028198242f },
            new Vector3 { X = 0.239693000912666f,    Y = 0.207875996828079f,    Z = 0.948332011699677f },
            new Vector3 { X = 0.012622999958694f,    Y = 0.258531987667084f,    Z = 0.965919971466065f },
            new Vector3 { X = -0.100556999444962f,   Y = 0.457147002220154f,    Z = 0.883687973022461f },
            new Vector3 { X = 0.0469669997692108f,   Y = 0.628588020801544f,    Z = 0.776319026947021f },
            new Vector3 { X = -0.430391013622284f,   Y = -0.445405006408691f,   Z = 0.785097002983093f },
            new Vector3 { X = -0.434291005134583f,   Y = -0.196227997541428f,   Z = 0.879139006137848f },
            new Vector3 { X = -0.256637006998062f,   Y = -0.33686700463295f,    Z = 0.905902028083801f },
            new Vector3 { X = -0.131372004747391f,   Y = -0.158910006284714f,   Z = 0.978514015674591f },
            new Vector3 { X = 0.102379001677036f,    Y = -0.208766996860504f,   Z = 0.972591996192932f },
            new Vector3 { X = 0.195686995983124f,    Y = -0.450129002332687f,   Z = 0.871258020401001f },
            new Vector3 { X = 0.627318978309631f,    Y = -0.42314800620079f,    Z = 0.653770983219147f },
            new Vector3 { X = 0.687439024448395f,    Y = -0.171582996845245f,   Z = 0.70568197965622f },
            new Vector3 { X = 0.275920003652573f,    Y = -0.021254999563098f,   Z = 0.960946023464203f },
            new Vector3 { X = 0.459367007017136f,    Y = 0.157465994358063f,    Z = 0.874177992343903f },
            new Vector3 { X = 0.285394996404648f,    Y = 0.583184003829956f,    Z = 0.760555982589722f },
            new Vector3 { X = -0.812174022197723f,   Y = 0.460303008556366f,    Z = 0.358460992574692f },
            new Vector3 { X = -0.189068004488945f,   Y = 0.641223013401032f,    Z = 0.743698000907898f },
            new Vector3 { X = -0.338874995708466f,   Y = 0.476480007171631f,    Z = 0.811251997947693f },
            new Vector3 { X = -0.920993983745575f,   Y = 0.347185999155045f,    Z = 0.176726996898651f },
            new Vector3 { X = 0.0406389981508255f,   Y = 0.024465000256896f,    Z = 0.998874008655548f },
            new Vector3 { X = -0.739131987094879f,   Y = -0.353747010231018f,   Z = 0.573189973831177f },
            new Vector3 { X = -0.603511989116669f,   Y = -0.286615014076233f,   Z = 0.744059979915619f },
            new Vector3 { X = -0.188675999641418f,   Y = -0.547058999538422f,   Z = 0.815554022789001f },
            new Vector3 { X = -0.0260450001806021f,  Y = -0.397819995880127f,   Z = 0.917093992233276f },
            new Vector3 { X = 0.267897009849548f,    Y = -0.649040997028351f,   Z = 0.712023019790649f },
            new Vector3 { X = 0.518245995044708f,    Y = -0.28489100933075f,    Z = 0.806385993957519f },
            new Vector3 { X = 0.493450999259949f,    Y = -0.0665329992771149f,  Z = 0.867224991321564f },
            new Vector3 { X = -0.328188002109528f,   Y = 0.140250995755196f,    Z = 0.934143006801605f },
            new Vector3 { X = -0.328188002109528f,   Y = 0.140250995755196f,    Z = 0.934143006801605f },
            new Vector3 { X = -0.328188002109528f,   Y = 0.140250995755196f,    Z = 0.934143006801605f },
            new Vector3 { X = -0.328188002109528f,   Y = 0.140250995755196f,    Z = 0.934143006801605f }
        };
        static Vector3[] tsNormals = new Vector3[36]
        {
            new Vector3 { X = 0.671213984489441f,   Y = 0.198492005467415f,     Z = -0.714193999767303f },
            new Vector3 { X = 0.269643008708954f,   Y = 0.584393978118897f,     Z = -0.765359997749329f },
            new Vector3 { X = -0.0405460000038147f, Y = 0.0969879999756813f,    Z = -0.994458973407745f },
            new Vector3 { X = -0.572427988052368f,  Y = -0.0919139981269836f,   Z = -0.814786970615387f },
            new Vector3 { X = -0.171400994062424f,  Y = -0.572709977626801f,    Z = -0.801639020442963f },
            new Vector3 { X = 0.362556993961334f,   Y = -0.30299898982048f,     Z = -0.881331026554108f },
            new Vector3 { X = 0.810347020626068f,   Y = -0.348971992731094f,    Z = -0.470697999000549f },
            new Vector3 { X = 0.103961996734142f,   Y = 0.938672006130218f,     Z = -0.328767001628876f },
            new Vector3 { X = -0.32404699921608f,   Y = 0.587669014930725f,     Z = -0.741375982761383f },
            new Vector3 { X = -0.800864994525909f,  Y = 0.340460985898972f,     Z = -0.492646992206574f },
            new Vector3 { X = -0.665498018264771f,  Y = -0.590147018432617f,    Z = -0.456988990306854f },
            new Vector3 { X = 0.314767003059387f,   Y = -0.803001999855042f,    Z = -0.506072998046875f },
            new Vector3 { X = 0.972629010677338f,   Y = 0.151076003909111f,     Z = -0.176550000905991f },
            new Vector3 { X = 0.680290997028351f,   Y = 0.684235990047455f,     Z = -0.262726992368698f },
            new Vector3 { X = -0.520079016685486f,  Y = 0.827777028083801f,     Z = -0.210482999682426f },
            new Vector3 { X = -0.961643993854523f,  Y = -0.179001003503799f,    Z = -0.207846999168396f },
            new Vector3 { X = -0.262713998556137f,  Y = -0.937451004981995f,    Z = -0.228401005268097f },
            new Vector3 { X = 0.219706997275352f,   Y = -0.971301019191742f,    Z = 0.0911249965429306f },
            new Vector3 { X = 0.923807978630066f,   Y = -0.229975000023842f,    Z = 0.306086987257004f },
            new Vector3 { X = -0.0824889987707138f, Y = 0.970659971237183f,     Z = 0.225866004824638f },
            new Vector3 { X = -0.591798007488251f,  Y = 0.696789979934692f,     Z = 0.405288994312286f },
            new Vector3 { X = -0.925296008586884f,  Y = 0.36660099029541f,      Z = 0.0971110016107559f },
            new Vector3 { X = -0.705051004886627f,  Y = -0.687775015830994f,    Z = 0.172828003764153f },
            new Vector3 { X = 0.732400000095367f,   Y = -0.680366992950439f,    Z = -0.0263049993664026f },
            new Vector3 { X = 0.855162024497986f,   Y = 0.37458199262619f,      Z = 0.358310997486114f },
            new Vector3 { X = 0.473006010055542f,   Y = 0.836480021476746f,     Z = 0.276704996824265f },
            new Vector3 { X = -0.0976170003414154f, Y = 0.654111981391907f,     Z = 0.750072002410889f },
            new Vector3 { X = -0.904124021530151f,  Y = -0.153724998235703f,    Z = 0.398658007383347f },
            new Vector3 { X = -0.211915999650955f,  Y = -0.858089983463287f,    Z = 0.467732012271881f },
            new Vector3 { X = 0.500226974487305f,   Y = -0.67440801858902f,     Z = 0.543090999126434f },
            new Vector3 { X = 0.584538996219635f,   Y = -0.110248997807503f,    Z = 0.8038409948349f },
            new Vector3 { X = 0.437373012304306f,   Y = 0.454643994569778f,     Z = 0.775888979434967f },
            new Vector3 { X = -0.0424409992992878f, Y = 0.0833180025219917f,    Z = 0.995618999004364f },
            new Vector3 { X = -0.596251010894775f,  Y = 0.220131993293762f,     Z = 0.772028028964996f },
            new Vector3 { X = -0.50645500421524f,   Y = -0.396977007389069f,    Z = 0.765448987483978f },
            new Vector3 { X = 0.0705690011382103f,  Y = -0.478473991155624f,    Z = 0.875262022018433f }
        };
        #endregion



        float scale = 4f / 50f;

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public Vxl2MdlMode Mode
        {
            get;
            set;
        }
        public Palette Palette
        {
            get;
            set;
        }
        public HVA HVA
        {
            get;
            set;
        }


        bool CheckVertex(VxlFlag[][][] flagsTable, VoxelData[][][] data, int i, int j, int k)
        {
            //BoundFlag flag = BoundFlag.None;
            bool iIn = i < data.Length;
            bool jIn = j < data[0].Length;
            bool kIn = k < data[0][0].Length;
            //if (i < sounds.Length)
            //    flag |= BoundFlag.ILL;
            //if (j < sounds[0].Length)
            //    flag |= BoundFlag.JLL;
            //if (k < sounds[0][0].Length)
            //    flag |= BoundFlag.KLL;

            //if (i>0)
            //    flag |= BoundFlag.

            if (kIn)
            {
                if (i > 0)
                {
                    if (j > 0 && data[i - 1][j - 1][k].used && flagsTable[i - 1][j - 1][k] != VxlFlag.None)
                        return true;
                    if (jIn && data[i - 1][j][k].used && flagsTable[i - 1][j][k] != VxlFlag.None)
                        return true;
                }

                if (iIn)
                {
                    if (j > 0 && data[i][j - 1][k].used && flagsTable[i][j - 1][k] != VxlFlag.None)
                        return true;

                    if (jIn && data[i][j][k].used && flagsTable[i][j][k] != VxlFlag.None)
                        return true;
                }

            }

            if (k > 0)
            {
                k--;
                if (i > 0)
                {
                    if (j > 0 && data[i - 1][j - 1][k].used && flagsTable[i - 1][j - 1][k] != VxlFlag.None)
                        return true;
                    if (jIn && data[i - 1][j][k].used && flagsTable[i - 1][j][k] != VxlFlag.None)
                        return true;
                }

                if (iIn)
                {
                    if (j > 0 && data[i][j - 1][k].used && flagsTable[i][j - 1][k] != VxlFlag.None)
                        return true;

                    if (jIn && data[i][j][k].used && flagsTable[i][j][k] != VxlFlag.None)
                        return true;
                }


            }

            return false;
        }



        VxlFlag[][][] CreatePassedTable(int ecx, int ecy, int ecz)
        {
            VxlFlag[][][] passed = new VxlFlag[ecx][][];
            for (int i = 0; i < ecx; i++)
            {
                passed[i] = new VxlFlag[ecy][];
                for (int j = 0; j < ecy; j++)
                {
                    passed[i][j] = new VxlFlag[ecz];
                }
            }
            return passed;
        }
        VoxelData[][][] GetData(VoxelSection sect, int ecx, int ecy, int ecz)
        {
            VoxelData[][][] data = new VoxelData[ecx][][];// sect.Data;

            for (int i = 0; i < ecx; i++)
            {
                data[i] = new VoxelData[ecy][];
                for (int j = 0; j < ecy; j++)
                {
                    data[i][j] = new VoxelData[ecz];

                    if (i >= 1 && j >= 1 && i < ecx - 1 && j < ecy - 1)
                    {
                        Array.Copy(sect.Data[i - 1][j - 1], 0, data[i][j], 1, ecz - 2);
                    }
                }
            }
            return data;
        }

        void MarkVxlFlags(VoxelData[][][] data, VxlFlag[][][] passed, int ecx, int ecy, int ecz)
        {
            Queue<Vector3i> q = new Queue<Vector3i>(ecx * ecy * ecz);
            q.Enqueue(new Vector3i(0, 0, 0));
            passed[0][0][0] = VxlFlag.Passed;

            VxlFlag[] stateEnumFlag = new VxlFlag[]
                {
                    VxlFlag.FacePosX, VxlFlag.FacePosY, VxlFlag.FacePosZ, 
                    VxlFlag.FaceNegX, VxlFlag.FaceNegY, VxlFlag.FaceNegZ
                };
            Vector3i[] stateEnum = new Vector3i[]
                {
                    new Vector3i(-1, 0, 0), new Vector3i(0, -1, 0), new Vector3i(0, 0, -1),
                    new Vector3i(1, 0, 0), new Vector3i(0, 1, 0), new Vector3i(0, 0, 1)
                };

            while (q.Count > 0)
            {
                Vector3i pos = q.Dequeue();

                for (int j = 0; j < 6; j++)
                {
                    Vector3i ns = pos;
                    ns.X += stateEnum[j].X;
                    ns.Y += stateEnum[j].Y;
                    ns.Z += stateEnum[j].Z;

                    if (ns.X >= 0 && ns.Y >= 0 && ns.Z >= 0 &&
                        ns.X < ecx && ns.Y < ecy && ns.Z < ecz)
                    {
                        if (!data[ns.X][ns.Y][ns.Z].used)
                        {
                            if ((passed[ns.X][ns.Y][ns.Z] & VxlFlag.Passed) == 0)
                            {
                                passed[ns.X][ns.Y][ns.Z] = VxlFlag.Passed;
                                q.Enqueue(ns);
                            }
                        }
                        else
                        {
                            if ((passed[ns.X][ns.Y][ns.Z] & stateEnumFlag[j]) == 0)
                            {
                                passed[ns.X][ns.Y][ns.Z] |= stateEnumFlag[j];
                            }
                        }
                    }
                } // for state enum
            }

        }

        Vector3[][][] CreateSmoothedPosition( Vector3[][][] buffer,VoxelData[][][] data, VxlFlag[][][] passed, int ecx, int ecy, int ecz)
        {
            int vx = ecx + 1;
            int vy = ecy + 1;
            int vz = ecz + 1;

            Vector3[][][] smoothed = new Vector3[vx][][];
            for (int i = 0; i < vx; i++)
            {
                smoothed[i] = new Vector3[vy][];
                for (int j = 0; j < vy; j++)
                {
                    smoothed[i][j] = new Vector3[vz];

                    //Array.Copy(buffer[i][j], smoothed[i][j], vz);

                    for (int k = 0; k < vz; k++)
                    {
                        smoothed[i][j][k] = buffer[i][j][k];
                        if (i > 0 && j > 0 && k > 0 &&
                            i < ecx && j < ecy && k < ecz)
                        {
                            if (CheckVertex(passed, data, i, j, k))
                            {
                                int count = 0;
                                Vector3 avgPos = new Vector3();

                                for (int a = -1; a < 2; a++)
                                    for (int b = -1; b < 2; b++)
                                        for (int c = -1; c < 2; c++)
                                        {
                                            int ii = i + a;
                                            int jj = j + b;
                                            int kk = k + c;

                                            if (CheckVertex(passed, data, ii, jj, kk))
                                            {
                                                count++;
                                                avgPos.X += buffer[ii][jj][kk].X;
                                                avgPos.Y += buffer[ii][jj][kk].Y;
                                                avgPos.Z += buffer[ii][jj][kk].Z;
                                            }
                                        }

                                if (count > 1)
                                {

                                    avgPos.X -= buffer[i][j][k].X;
                                    avgPos.Y -= buffer[i][j][k].Y;
                                    avgPos.Z -= buffer[i][j][k].Z;

                                    float inv = 1f / (float)(count - 1);
                                    avgPos.X *= inv; avgPos.Y *= inv; avgPos.Z *= inv;

                                    //Vector3 offset = new Vector3(avgPos.X - buffer[i][j][k].X, avgPos.Y - buffer[i][j][k].Y, avgPos.Z - buffer[i][j][k].Z);

                                    //avgPos.X *= 0.5f; avgPos.Y *= 0.5f; avgPos.Z *= 0.5f;


                                    avgPos.X += 0.5f * (avgPos.X - buffer[i][j][k].X);
                                    avgPos.Y += 0.5f * (avgPos.Y - buffer[i][j][k].Y);
                                    avgPos.Z += 0.5f * (avgPos.Z - buffer[i][j][k].Z);

                                    smoothed[i][j][k] = avgPos;
                                }
                                else
                                {
                                    smoothed[i][j][k] = buffer[i][j][k];
                                }
                            }
                        }



                    }
                }
            }
            return smoothed;
        }
        Vector3[][][] CreateSmoothedPosition(VoxelData[][][] data, VxlFlag[][][] passed, int ecx, int ecy, int ecz) 
        {
            int vx = ecx + 1;
            int vy = ecy + 1;
            int vz = ecz + 1;

            Vector3[][][] buffer = CreatePosition(data, passed, ecx, ecy, ecz);
 
            Vector3[][][] smoothed = new Vector3[vx][][];
            for (int i = 0; i < vx; i++)
            {
                smoothed[i] = new Vector3[vy][];
                for (int j = 0; j < vy; j++)
                {
                    smoothed[i][j] = new Vector3[vz];

                    //Array.Copy(buffer[i][j], smoothed[i][j], vz);

                    for (int k = 0; k < vz; k++)
                    {
                        smoothed[i][j][k] = buffer[i][j][k];
                        if (i > 0 && j > 0 && k > 0 &&
                            i < ecx && j < ecy && k < ecz)
                        {
                            if (CheckVertex(passed, data, i, j, k))
                            {
                                int count = 0;
                                Vector3 avgPos = new Vector3();

                                for (int a = -1; a < 2; a++)
                                    for (int b = -1; b < 2; b++)
                                        for (int c = -1; c < 2; c++)
                                        {
                                            int ii = i + a;
                                            int jj = j + b;
                                            int kk = k + c;

                                            if (CheckVertex(passed, data, ii, jj, kk))
                                            {
                                                count++;
                                                avgPos.X += buffer[ii][jj][kk].X;
                                                avgPos.Y += buffer[ii][jj][kk].Y;
                                                avgPos.Z += buffer[ii][jj][kk].Z;
                                            }
                                        }

                                if (count > 1)
                                {

                                    avgPos.X -= buffer[i][j][k].X;
                                    avgPos.Y -= buffer[i][j][k].Y;
                                    avgPos.Z -= buffer[i][j][k].Z;

                                    float inv = 1f / (float)(count - 1);
                                    avgPos.X *= inv; avgPos.Y *= inv; avgPos.Z *= inv;

                                    //Vector3 offset = new Vector3(avgPos.X - buffer[i][j][k].X, avgPos.Y - buffer[i][j][k].Y, avgPos.Z - buffer[i][j][k].Z);

                                    //avgPos.X *= 0.5f; avgPos.Y *= 0.5f; avgPos.Z *= 0.5f;


                                    avgPos.X += 0.5f * (avgPos.X - buffer[i][j][k].X);
                                    avgPos.Y += 0.5f * (avgPos.Y - buffer[i][j][k].Y);
                                    avgPos.Z += 0.5f * (avgPos.Z - buffer[i][j][k].Z);

                                    smoothed[i][j][k] = avgPos;
                                }
                                else
                                {
                                    smoothed[i][j][k] = buffer[i][j][k];
                                }
                            }
                        }



                    }
                }
            }

            for (int i = 0; i < vx; i++)
            {
                for (int j = 0; j < vy; j++)
                {
                    buffer[i][j] = null;
                }
                buffer[i] = null;
            }
            buffer = null;

            return smoothed;
        }
        Vector3[][][] CreatePosition(VoxelData[][][] data, VxlFlag[][][] passed, int ecx, int ecy, int ecz)
        {
            int vx = ecx + 1;
            int vy = ecy + 1;
            int vz = ecz + 1;

            Vector3[][][] buffer = new Vector3[vx][][];
            for (int i = 0; i < vx; i++)
            {
                buffer[i] = new Vector3[vy][];
                for (int j = 0; j < vy; j++)
                {
                    buffer[i][j] = new Vector3[vz];
                    for (int k = 0; k < vz; k++)
                    {
                        if (i < ecx && j < ecy && k < ecz)
                        {
                            buffer[i][j][k] = new Vector3(i * scale, j * scale, k * scale);
                        }
                        else
                        {
                            int ii = i;
                            int jj = j;
                            int kk = k;
                            if (ii == ecx)
                                ii--;

                            if (jj == ecy)
                                jj--;

                            if (kk == ecz)
                                kk--;

                            buffer[i][j][k] = new Vector3(i * scale, j * scale, k * scale);

                        }
                    }
                }
            }

            return buffer;
        }

        bool IsAdjacent(ref VxlFace a, ref VxlFace b, out int ea)
        {
            bool result = false;
            ea = -1;
            //eb = -1;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    int ea1 = i;
                    int ea2 = i == 3 ? 0 : i + 1;

                    int eb1 = j;
                    int eb2 = j == 3 ? 0 : j + 1;

                    if ((a.vtx[ea1] == b.vtx[eb1] && a.vtx[ea2] == b.vtx[eb2]) ||
                        (a.vtx[ea1] == b.vtx[eb2] && a.vtx[ea2] == b.vtx[eb1]))
                    {
                        ea = i;
                        result = true;
                        break;
                    }
                }
            return result;
        }
        //AdjFaceInfo[][] CreateAdjFaceInfo(VxlFlag[] stateFlag, Vector3i[][] stateEnum)
        //{
        //    Vector3i[] vxlPos = new Vector3i[27];
        //    VxlFace[] faces = new VxlFace[27 * 6];

        //    int index = 0;
        //    int index2 = 0;
        //    for (int a = -1; a < 2; a++)
        //        for (int b = -1; b < 2; b++)
        //            for (int c = -1; c < 2; c++)
        //            {
        //                for (int k = 0; k < 6; k++)
        //                {
        //                    faces[index] = new VxlFace();
        //                    for (int m = 0; m < 4; m++)
        //                    {
        //                        faces[index].vtx[m] = stateEnum[k][m];

        //                        faces[index].vtx[m].X += (short)a;
        //                        faces[index].vtx[m].Y += (short)b;
        //                        faces[index].vtx[m].Z += (short)c;
        //                    }
        //                    index++;
        //                }
        //                vxlPos[index2++] = new Vector3i(a, b, c);
        //            }

        //    VxlFace[] oface = new VxlFace[6];
        //    for (int k = 0; k < 6; k++)
        //    {
        //        oface[k] = new VxlFace();
        //        for (int m = 0; m < 4; m++)
        //        {
        //            oface[k].vtx[m] = stateEnum[k][m];
        //        }
        //    }

        //    AdjFaceInfo[][] adjFaces = new AdjFaceInfo[6][];
        //    for (int i = 0; i < 6; i++)
        //    {
        //        List<AdjFaceInfo> adjs = new List<AdjFaceInfo>(25);
        //        for (int j = 0; j < 27 * 6; j++)
        //        {
        //            if (oface[i] != faces[j])
        //            {
        //                int edge;
        //                if (IsAdjacent(ref oface[i], ref faces[j], out edge))
        //                {
        //                    AdjFaceInfo faceAdd;
        //                    faceAdd.face = stateFlag[j % 6];
        //                    faceAdd.vxlPos = vxlPos[j / 6];
        //                    adjs.Add(faceAdd);


        //                }
        //            }
        //        }
        //        adjFaces[i] = adjs.ToArray();
        //    }

        //    return adjFaces;
        //}

        void CreateAdjFaceInfo(VxlFlag[] stateFlag, Vector3i[][] stateEnum, out AdjFaceInfo[][] adjFaces, out int[][] adjEdge)
        {
            Vector3i[] vxlPos = new Vector3i[27];
            VxlFace[] faces = new VxlFace[27 * 6];

            int index = 0;
            int index2 = 0;
            for (int a = -1; a < 2; a++)
                for (int b = -1; b < 2; b++)
                    for (int c = -1; c < 2; c++)
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            faces[index] = new VxlFace();
                            for (int m = 0; m < 4; m++)
                            {
                                faces[index].vtx[m] = stateEnum[k][m];

                                faces[index].vtx[m].X += (short)a;
                                faces[index].vtx[m].Y += (short)b;
                                faces[index].vtx[m].Z += (short)c;
                            }
                            index++;
                        }
                        vxlPos[index2++] = new Vector3i(a, b, c);
                    }

            VxlFace[] oface = new VxlFace[6];
            for (int k = 0; k < 6; k++)
            {
                oface[k] = new VxlFace();
                for (int m = 0; m < 4; m++)
                {
                    oface[k].vtx[m] = stateEnum[k][m];
                }
            }

            adjFaces = new AdjFaceInfo[6][];
            adjEdge = new int[6][];
            for (int i = 0; i < 6; i++)
            {
                // faces adjacent with oface[i]
                List<AdjFaceInfo> adjs = new List<AdjFaceInfo>(25);
                // adjacent edges in oface[i]
                List<int> adjs2 = new List<int>(25);
                for (int j = 0; j < 27 * 6; j++)
                {
                    if (oface[i] != faces[j])
                    {
                        int edge;
                        if (IsAdjacent(ref oface[i], ref faces[j], out edge))
                        {
                            AdjFaceInfo faceAdd;
                            faceAdd.face = stateFlag[j % 6];
                            faceAdd.vxlPos = vxlPos[j / 6];
                            adjs.Add(faceAdd);
                            adjs2.Add(edge);
                        }
                    }
                }
                adjFaces[i] = adjs.ToArray();
                adjEdge[i] = adjs2.ToArray();
            }
        }

        
        
        int GetFaceHashCode(int x, int y, int z, VxlFlag flag)
        {
            return ((int)flag) << 24 | (x << 16) | (y << 8) | z;
        }

        int[][] EdgeTexture(int w, int h, int* src)
        {
            Point[] lerpEnum = new Point[8]
                {
                    new Point(-1, 0),
                    new Point(1, 0),
                    new Point(0, 1),
                    new Point(0, -1),

                    new Point(-1, -1),
                    new Point(1, 1),
                    new Point(1, -1),
                    new Point(-1, 1)
                };

            int[][] texBuffer = new int[h][];
            for (int i = 0; i < h; i++)
            {
                texBuffer[i] = new int[w];
                for (int j = 0; j < w; j++)
                {
                    int offset = i * w + j;
                    texBuffer[i][j] = src[offset];
                    if (texBuffer[i][j] == 0)
                    {
                        int avrR = 0;
                        int avrG = 0;
                        int avrB = 0;
                        int count = 0;

                        for (int k = 0; k < 8; k++)
                        {
                            int px = j + lerpEnum[k].X;
                            int py = i + lerpEnum[k].Y;

                            int offset2 = py * w + px;

                            if (px >= 0 && py >= 0 && px < w && py < h &&
                                src[offset2] != 0)
                            {
                                count++;
                                avrR += (src[offset2] & 0x00ff0000) >> 16;
                                avrG += (src[offset2] & 0x0000ff00) >> 8;
                                avrB += (src[offset2] & 0x000000ff);
                            }
                        }

                        if (count > 0)
                        {
                            avrR /= count;
                            avrG /= count;
                            avrB /= count;
                            texBuffer[i][j] = (0xff << 24) | (avrR << 16) | (avrG << 8) | (avrB);
                        }
                    }
                }
            }
            return texBuffer;
        }

        public unsafe override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            HVA hva;

            VxlConvDlg dlg = new VxlConvDlg();
            dlg.ShowDialog();

            if (dlg.DialogResult == DialogResult.OK)
            {
                //hva = new HVA(new DevFileLocation(dlg.HVAFile));
                Palette = Palette.FromFile(new DevFileLocation(dlg.PaletteFile));
            }
            else
            {
                dlg.Dispose();
                return;
            }

            //EditableModel texSrc = null;
            bool smooth = dlg.Smooth;

            dlg.Dispose();


            VxlFlag[] stateFlag = new VxlFlag[6]
            {
                VxlFlag.FaceNegX,
                VxlFlag.FaceNegY,
                VxlFlag.FaceNegZ,
                VxlFlag.FacePosX,
                VxlFlag.FacePosY,
                VxlFlag.FacePosZ
            };
            Vector3i[][] stateEnum = new Vector3i[6][]
            {
                new Vector3i[4]
                {
                    new Vector3i {X = 0, Y = 0, Z = 0},
                    new Vector3i {X = 0, Y = 1, Z = 0},
                    new Vector3i {X = 0, Y = 1, Z = 1},
                    new Vector3i {X = 0, Y = 0, Z = 1}
                }, 
                new Vector3i[4]
                {
                    new Vector3i {X = 0, Y = 0, Z = 0},
                    new Vector3i {X = 1, Y = 0, Z = 0},
                    new Vector3i {X = 1, Y = 0, Z = 1},
                    new Vector3i {X = 0, Y = 0, Z = 1}
                }, 
                new Vector3i[4]
                {
                    new Vector3i {X = 0, Y = 0, Z = 0},
                    new Vector3i {X = 1, Y = 0, Z = 0},
                    new Vector3i {X = 1, Y = 1, Z = 0},
                    new Vector3i {X = 0, Y = 1, Z = 0}
                }, 

                new Vector3i[4]
                {
                    new Vector3i {X = 1, Y = 0, Z = 0},
                    new Vector3i {X = 1, Y = 1, Z = 0},
                    new Vector3i {X = 1, Y = 1, Z = 1},
                    new Vector3i {X = 1, Y = 0, Z = 1}
                }, 
                new Vector3i[4]
                {
                    new Vector3i {X = 0, Y = 1, Z = 0},
                    new Vector3i {X = 1, Y = 1, Z = 0},
                    new Vector3i {X = 1, Y = 1, Z = 1},
                    new Vector3i {X = 0, Y = 1, Z = 1}
                }, 
                new Vector3i[4]
                {
                    new Vector3i {X = 0, Y = 0, Z = 1},
                    new Vector3i {X = 1, Y = 0, Z = 1},
                    new Vector3i {X = 1, Y = 1, Z = 1},
                    new Vector3i {X = 0, Y = 1, Z = 1}
                }
            };

            // adjacent faces of 6 faces on a cube
            AdjFaceInfo[][] adjFaces;
            // adjacent edges in each face on a cube
            int[][] adjEdges;

            CreateAdjFaceInfo(stateFlag, stateEnum, out adjFaces, out adjEdges);


            Voxel vxl = new Voxel(source);

            VoxelSection[] sects = vxl.Section;

            EditableMesh[] meshes = new EditableMesh[sects.Length];
            Matrix[] trans = new Matrix[sects.Length];

            for (int s = 0; s < sects.Length; s++)
            {
                VoxelSection sect = sects[s];

                sect.GetTransform(out trans[s]);

                int cx = sect.XSize;
                int cy = sect.YSize;
                int cz = sect.ZSize;

                int ecx = cx + 2;
                int ecy = cy + 2;
                int ecz = cz + 2;


                VxlFlag[][][] passed = CreatePassedTable(ecx, ecy, ecz);

                VoxelData[][][] data = GetData(sect, ecx, ecy, ecz);

                MarkVxlFlags(data, passed, ecx, ecy, ecz);

                Vector3[][][] pos = CreatePosition(data, passed, ecx, ecy, ecz);
                Vector3[][][] smoothed;
                if (smooth)                
                    smoothed = CreateSmoothedPosition(data, passed, ecx, ecy, ecz);                
                else                
                    smoothed = pos;                

                // 
                List<MeshFace> faces = new List<MeshFace>(30000);
                // a list used to store all vertices
                List<VertexPT1> vertices = new List<VertexPT1>(30000);

                List<VxlQuad> quads = new List<VxlQuad>(30000);

                //List<Vector3> quadNormals = new List<Vector3>(30000);
                // for looking up a face's index in var:faces by its hashcode 
                Dictionary<int, VxlQuad> faceTable = new Dictionary<int, VxlQuad>(30000);

                Vector3[] normalTbl = Mode == Vxl2MdlMode.Ra2 ? ra2Normals : tsNormals;
                byte uBound = (byte)(normalTbl.Length - 1);

                int quadIndex = 0;
                for (int i = 0; i < ecx; i++)
                    for (int j = 0; j < ecy; j++)
                        for (int k = 0; k < ecz; k++)
                        {
                            if (data[i][j][k].normal > uBound)
                                data[i][j][k].normal = uBound;
                            byte fn = data[i][j][k].normal;
                            for (int m = 0; m < 6; m++)
                            {
                                if ((passed[i][j][k] & stateFlag[m]) != 0)
                                {
                                    int f1a = vertices.Count;
                                    int f1b = f1a + 1;
                                    int f1c = f1a + 2;
                                    int f2a = f1a;
                                    int f2b = f1a + 2;
                                    int f2c = f1a + 3;

                                    for (int p = 0; p < 4; p++)
                                    {
                                        VertexPT1 vtx = new VertexPT1();
                                        vtx.pos = smoothed[i + stateEnum[m][p].X][j + stateEnum[m][p].Y][k + stateEnum[m][p].Z];
                                        vertices.Add(vtx);
                                    }

                                    VxlQuad quad = new VxlQuad(quadIndex++);
                                    quad.normal = fn;
                                    quad.color = data[i][j][k].colour;
                                    // a quad is made up of 2 triangles(MeshFace)
                                    faceTable.Add(GetFaceHashCode(i, j, k, stateFlag[m]), quad);
                                    quads.Add(quad);
                                   
                                    faces.Add(new MeshFace(f1a, f1b, f1c, 0));
                                    faces.Add(new MeshFace(f2a, f2b, f2c, 0));                                   
                                }
                            }
                        }


                // generate adjacent info
                for (int i = 1; i <= cx; i++)
                    for (int j = 1; j <= cy; j++)
                        for (int k = 1; k <= cz; k++)
                        {
                            for (int m = 0; m < 6; m++)
                            {
                                if ((passed[i][j][k] & stateFlag[m]) != 0)
                                {
                                    VxlQuad quad = faceTable[GetFaceHashCode(i, j, k, stateFlag[m])];

                                    for (int h = 0; h < adjFaces[m].Length; h++)
                                    {
                                        int tx = i + adjFaces[m][h].vxlPos.X;
                                        int ty = j + adjFaces[m][h].vxlPos.Y;
                                        int tz = k + adjFaces[m][h].vxlPos.Z;

                                        if ((passed[tx][ty][tz] & adjFaces[m][h].face) != 0)
                                        {
                                            quad.adj[adjEdges[m][h]].Enqueue(faceTable[GetFaceHashCode(tx, ty, tz, adjFaces[m][h].face)]);
                                        }
                                    }
                                }
                            }
                        }


                const int MapLen = 512;
                ImageQuadTreeNode altas = new ImageQuadTreeNode(MapLen, MapLen, 8);
                Texture clrTex = new Texture(GraphicsDevice.Instance.Device, MapLen, MapLen, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                Texture norTex = new Texture(GraphicsDevice.Instance.Device, MapLen, MapLen, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                int* colorData = (int*)clrTex.LockRectangle(0, LockFlags.None).Data.DataPointer.ToPointer();
                int* norData = (int*)norTex.LockRectangle(0, LockFlags.None).Data.DataPointer.ToPointer();


                bool[] quadPassed = new bool[quads.Count];
                Queue<VxlQuad> queue = new Queue<VxlQuad>(quads.Count + 1);



                for (int i = 0; i < quads.Count; i++)
                {

                    if (!quadPassed[i])
                    { 
                        // used to check if a pixel is already used
                        Dictionary<int, VxlQuad> texHashTbl = new Dictionary<int, VxlQuad>(quads.Count);

                        queue.Enqueue(quads[i]);
                        quadPassed[i] = true;
                        quads[i].TexX = 0;
                        quads[i].TexY = 0;

                        int minX = int.MaxValue;
                        int minY = int.MaxValue;
                        int maxX = int.MinValue;
                        int maxY = int.MinValue;

                        while (queue.Count > 0)
                        {
                            VxlQuad quad = queue.Dequeue();


                            int hash = quad.GetHashCode();
                            if (!texHashTbl.ContainsKey(hash))
                            {
                                texHashTbl.Add(hash, quad);

                                if (quad.TexX < minX)
                                    minX = quad.TexX;
                                if (quad.TexY < minY)
                                    minY = quad.TexY;
                                if (quad.TexX > maxX)
                                    maxX = quad.TexX;
                                if (quad.TexY > maxY)
                                    maxY = quad.TexY;


                                //colorData[quad.TexY * MapLen + quad.TexX] = Palette.Data[quad.color].ToArgb();
                                //norData[quad.TexY * MapLen + quad.TexX] = (0xff << 24) | (((byte)(127f * quad.normal.X + 128f)) << 16) | (((byte)(127f * quad.normal.Y + 128f)) << 8) | ((byte)(127f * quad.normal.Z + 128f)); 
                                bool isEmpty = true;
                                for (int j = 0; j < 4; j++)
                                {
                                    Queue<VxlQuad> adjq = quad.adj[j];
                                    if (adjq.Count > 0)
                                    {
                                        VxlQuad nq = adjq.Dequeue();

                                        if (adjq.Count > 0)
                                            isEmpty = false;
                                        if (!quadPassed[nq.Index])
                                        {
                                            switch (j)
                                            {
                                                case 0:
                                                    nq.TexX = quad.TexX;
                                                    nq.TexY = (short)(quad.TexY - 1);
                                                    break;
                                                case 1:
                                                    nq.TexX = (short)(quad.TexX + 1);
                                                    nq.TexY = quad.TexY;
                                                    break;
                                                case 2:
                                                    nq.TexX = quad.TexX;
                                                    nq.TexY = (short)(quad.TexY + 1);
                                                    break;
                                                case 3:
                                                    nq.TexX = (short)(quad.TexX - 1);
                                                    nq.TexY = quad.TexY;
                                                    break;
                                            }

                                            queue.Enqueue(nq);
                                            quadPassed[nq.Index] = true;
                                        }
                                    }
                                }
                                if (!isEmpty)
                                {
                                    quadPassed[quad.Index] = false;
                                }
                            }
                            else
                            {
                                quadPassed[quad.Index] = false;
                            }
                            
                        }

                        int texW = maxX - minX + 1;
                        int texH = maxY - minY + 1;

                        //Texture clrTex = new Texture(GraphicsDevice.Instance.Device, texW, texH, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                        //Texture norTex = new Texture(GraphicsDevice.Instance.Device, texW, texH, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                        Point texPos;
                        if (!altas.Find(texW, texH, out texPos))
                        {
                            throw new OutOfMemoryException();
                        }


                        Dictionary<int, VxlQuad>.ValueCollection pixels = texHashTbl.Values;
                        foreach (VxlQuad vq in pixels)
                        {
                            int py = vq.TexY - minY + texPos.Y;
                            int px = vq.TexX - minX + texPos.X;

                            int offset = py * MapLen + px;

                            colorData[offset] = Palette.Data[vq.color].ToArgb();
                            Vector3 norm = normalTbl[vq.normal];
                            norData[offset] = MathEx.Vector2ARGB(ref norm);

                            int faceA = vq.Index * 2;
                            int faceB = faceA + 1;


                            VertexPT1 va = vertices[faces[faceA].IndexA];
                            VertexPT1 vb = vertices[faces[faceA].IndexB];
                            VertexPT1 vc = vertices[faces[faceA].IndexC];
                            VertexPT1 vd = vertices[faces[faceB].IndexC];

                            //va.u1 = ((float)px - 0.5f) / (float)MapLen;
                            //va.v1 = ((float)py - 0.5f) / (float)MapLen;

                            //vb.u1 = ((float)px + 0.5f) / (float)MapLen;
                            //vb.v1 = ((float)py - 0.5f) / (float)MapLen;

                            //vc.u1 = ((float)px + 0.5f) / (float)MapLen;
                            //vc.v1 = ((float)py + 0.5f) / (float)MapLen;

                            //vd.u1 = ((float)px - 0.5f) / (float)MapLen;
                            //vd.v1 = ((float)py + 0.5f) / (float)MapLen;
                            va.u1 = ((float)px) / (float)MapLen;
                            va.v1 = ((float)py) / (float)MapLen;

                            vb.u1 = ((float)px) / (float)MapLen;
                            vb.v1 = ((float)py) / (float)MapLen;

                            vc.u1 = ((float)px) / (float)MapLen;
                            vc.v1 = ((float)py) / (float)MapLen;

                            vd.u1 = ((float)px) / (float)MapLen;
                            vd.v1 = ((float)py) / (float)MapLen;

                            vertices[faces[faceA].IndexA] = va;
                            vertices[faces[faceA].IndexB] = vb;
                            vertices[faces[faceA].IndexC] = vc;
                            vertices[faces[faceB].IndexC] = vd;

                        }

                    }
                }


                int[][] texBuffer = EdgeTexture(MapLen, MapLen, colorData);
                for (int i = 0; i < MapLen; i++)
                {
                    fixed (void* src = &texBuffer[i][0])
                    {
                        Memory.Copy(src, colorData + i * MapLen, MapLen * 4);
                    }
                }
                texBuffer = EdgeTexture(MapLen, MapLen, colorData);
                for (int i = 0; i < MapLen; i++)
                {
                    fixed (void* src = &texBuffer[i][0])
                    {
                        Memory.Copy(src, colorData + i * MapLen, MapLen * 4);
                    }
                }


                texBuffer = EdgeTexture(MapLen, MapLen, norData);
                for (int i = 0; i < MapLen; i++)
                {
                    fixed (void* src = &texBuffer[i][0])
                    {
                        Memory.Copy(src, norData + i * MapLen, MapLen * 4);
                    }
                }
                texBuffer = EdgeTexture(MapLen, MapLen, norData);
                for (int i = 0; i < MapLen; i++)
                {
                    fixed (void* src = &texBuffer[i][0])
                    {
                        Memory.Copy(src, norData + i * MapLen, MapLen * 4);
                    }
                }


                clrTex.UnlockRectangle(0);
                norTex.UnlockRectangle(0);

                //Texture.ToFile(clrTex, @"C:\Documents and Settings\Yuri\桌面\testTex\c.png", ImageFileFormat.Png);
                //Texture.ToFile(norTex, @"C:\Documents and Settings\Yuri\桌面\testTex\n.png", ImageFileFormat.Png);


                EditableMesh mesh = new EditableMesh();

                Vector3[] positions = new Vector3[vertices.Count];
                //Vector3[] n = new Vector3[vertices.Count];
                Vector2[] tex1 = new Vector2[vertices.Count];

                for (int i = 0; i < vertices.Count; i++)
                {
                    VertexPT1 vtx = vertices[i];
                    positions[i] = vtx.pos;
                    //n[i] = vertices[i].n;
                    tex1[i] = new Vector2(vtx.u1, vtx.v1);
                }

                mesh.Positions = positions;
                //mesh.Normals = n;
                mesh.Texture1 = tex1;

                mesh.Name = sect.Name;

                EditableMeshMaterial mate = new EditableMeshMaterial();
                mate.mat = MeshMaterial.DefaultMatColor;
                mate.Texture1 = clrTex;
                mate.Texture2 = norTex;
                mate.EffectName = "NormapMapping";

                DevFileLocation fl = source as DevFileLocation;
                if (fl != null)
                {
                    mate.TextureFile1 = Path.GetFileNameWithoutExtension(fl.Path) + "c.png";
                    mate.TextureFile2 = Path.GetFileNameWithoutExtension(fl.Path) + "n.png";

                    DevFileLocation dfl = dest as DevFileLocation;

                    if (dfl != null)
                    {
                        string path = Path.GetDirectoryName(dfl.Path);
                        Texture.ToFile(clrTex, Path.Combine(path, mate.TextureFile1), ImageFileFormat.Png);
                        Texture.ToFile(norTex, Path.Combine(path, mate.TextureFile2), ImageFileFormat.Png);

                        mate.Texture1Embeded = false;
                        mate.Texture2Embeded = false;
                    }
                    else
                    {
                        mate.Texture1Embeded = true;
                        mate.Texture2Embeded = true;
                    }
                }
                else
                {
                    mate.Texture1Embeded = true;
                    mate.Texture2Embeded = true;
                }

                mesh.Materials = new EditableMeshMaterial[1] { mate };

                mesh.Faces = faces.ToArray();
                mesh.SetVertexFormat(VertexPT1.Format);

                mesh.WeldVertices(true, false, true);
                meshes[s] = mesh;
            }


            EditableModel mdl = new EditableModel();

#warning hva anim
            mdl.Entities = meshes;
            mdl.ModelAnimation = new NoAnimation(GraphicsDevice.Instance.Device, trans);

            EditableModel.ToFile(mdl, dest);

            mdl.Dispose();
        }



        public override void ShowDialog(object sender, EventArgs e)
        {
            string[] files;
            string path;
            if (ConvDlg.Show(Program.StringTable[CsfKey], GetOpenFilter(), out files, out path) == DialogResult.OK)
            {
                ProgressDlg pd = new ProgressDlg(Program.StringTable["GUI:Converting"]);

                pd.MinVal = 0;
                pd.Value = 0;
                pd.MaxVal = files.Length;

                pd.Show();
                for (int i = 0; i < files.Length; i++)
                {
                    string dest = Path.Combine(path, Path.GetFileNameWithoutExtension(files[i]) + ".mesh");

                    Convert(new DevFileLocation(files[i]), new DevFileLocation(dest));
                    pd.Value = i;
                }
                pd.Close();
                pd.Dispose();
            }
        }

        public override string Name
        {
            get { return Program.StringTable[CsfKey]; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".vxl" }; }
        }
        public override string[] DestExt
        {
            get { return new string[] { ".mesh" }; }
        }
        public override string DestDesc
        {
            get { return Program.StringTable["DOCS:MESHDESC"]; }
        }
        public override string SourceDesc
        {
            get { return Program.StringTable["DOCS:VxlDesc"]; }
        }
     }
}
