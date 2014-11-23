using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace KurisuRiven
{
    internal struct Coordinate
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public Coordinate(Vector3 p1, Vector3 p2)
        {
            pointA = p1;
            pointB = p2;
        }
    }

    internal class KurisuLib
    {      
        private static bool active;
        public static bool canexport = true;

        private const int minrange = 100;
        private const int rotatemultiplier = 15;
        private static Vector3 startpoint, endpoint, directionpoint;

        private static readonly Spell _q = new Spell(SpellSlot.Q, 280f);
        private static readonly Obj_AI_Hero player = ObjectManager.Player;

        public KurisuLib()
        {
            Coordinates();
            Game.OnGameUpdate += Game_OnGameUpdate;           
        }

        public static readonly List<Coordinate> Jumplist = new List<Coordinate>();

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (KurisuRiven.config.Item("exportjump").GetValue<KeyBind>().Active && canexport)
            {
                using (var file = new System.IO.StreamWriter(@"C:\Rivenjumps.txt"))
                {
                    file.WriteLine(player.Position.X + " " + player.Position.Y + " " + player.Position.Z);
                    file.Close();
                }

                Game.PrintChat("Debug: Position exported!");
                canexport = false;
            }
            else
            {
                canexport = true;
            }

            if (!active && KurisuRiven.config.Item("jumpkey").GetValue<KeyBind>().Active && KurisuRiven.cleavecount == 2)
            {
                var playeradjacentpos = (float) (minrange + 1);
                for (int i = 0; i < Jumplist.Count; i++)
                {
                    Coordinate pos = Jumplist[i];
                    if (player.Distance(pos.pointA) < playeradjacentpos || player.Distance(pos.pointB) < playeradjacentpos)
                    {
                        active = true;
                        if (player.Distance(pos.pointA) < player.Distance(pos.pointB))
                        {
                            playeradjacentpos = player.Distance(pos.pointA);
                            startpoint = pos.pointA;
                            endpoint = pos.pointB;
                        }
                        else
                        {
                            playeradjacentpos = player.Distance(pos.pointB);
                            startpoint = pos.pointB;
                            endpoint = pos.pointA;
                        }
                    }
                }
                if (active)
                {
                    directionpoint.X = startpoint.X - endpoint.X;
                    directionpoint.Y = startpoint.Y - endpoint.Y;

                    player.IssueOrder(GameObjectOrder.HoldPosition, player.ServerPosition);
                    Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(startpoint.X, startpoint.Y)).Send();
                    Utility.DelayAction.Add(Game.Ping + 70, Direction1);
                }
            }         

        }


        private void Direction1()
        {
            player.IssueOrder(GameObjectOrder.HoldPosition, player.ServerPosition);
            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(startpoint.X + directionpoint.X/rotatemultiplier,
                startpoint.Y + directionpoint.Y/rotatemultiplier)).Send();

            //directionpos = new Vector3(startpoint.X, startpoint.Y, startpoint.Z)
            //{
            //    X = startpoint.X + directionpoint.X/rotatemultiplier,
            //    Y = startpoint.Y + directionpoint.Y/rotatemultiplier,
            //    Z = startpoint.Z + directionpoint.Z/rotatemultiplier
            //};

            Utility.DelayAction.Add(Game.Ping + 17, Direction2);
        }

        private void Direction2()
        {
            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(startpoint.X, startpoint.Y)).Send();
            Utility.DelayAction.Add(Game.Ping + 23, Walljump);
        }

        public static void Walljump()
        {
            _q.Cast(endpoint, true);
            player.IssueOrder(GameObjectOrder.HoldPosition, player.ServerPosition);
            Utility.DelayAction.Add(1000, () => active = false);
        }

        private static void Coordinates()
        {
            
            Jumplist.Add(new Coordinate(new Vector3(7873f,
                8735f,
                55.44274f), new Vector3(7545f, 9063f, 55.6065f)));
            //Jumplist.Add(new Coordinate(new Vector3(7593f,
            //    9065f,
            //    55.60519f), new Vector3(7873f, 9065f,)));
            Jumplist.Add(new Coordinate(new Vector3(6393.7299804688f,
                -63.87451171875f,
                8341.7451171875f), new Vector3
                    (6612.1625976563f,
                        56.018413543701f,
                        8574.7412109375f
                    )
                ));

            Jumplist.Add(new Coordinate(new Vector3(7041.7885742188f,
                8810.1787109375f,
                0f), new Vector3
                    (7296.0341796875f,
                        9056.4638671875f,
                        55.610824584961f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4546.0258789063f,
                2548.966796875f,
                54.257415771484f), new Vector3
                    (4185.0786132813f,
                        2526.5520019531f,
                        109.35539245605f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2805.4074707031f,
                6140.130859375f,
                55.182941436768f), new Vector3
                    (2614.3215332031f,
                        5816.9438476563f,
                        60.193073272705f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6696.486328125f,
                5377.4013671875f,
                61.310482025146f), new Vector3
                    (6868.6918945313f,
                        5698.1455078125f,
                        55.616455078125f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(1677.9854736328f,
                8319.9345703125f,
                54.923847198486f), new Vector3
                    (1270.2786865234f,
                        8286.544921875f,
                        50.334892272949f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2809.3254394531f,
                10178.6328125f,
                -58.759708404541f), new Vector3
                    (2553.8962402344f,
                        9974.4677734375f,
                        53.364395141602f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5102.642578125f,
                10322.375976563f,
                -62.845260620117f), new Vector3
                    (5483f,
                        10427,
                        54.5009765625f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6000.2373046875f,
                11763.544921875f,
                39.544124603271f), new Vector3
                    (6056.666015625f,
                        11388.752929688f,
                        54.385917663574f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(1742.34375f,
                7647.1557617188f,
                53.561042785645f), new Vector3
                    (1884.5321044922f,
                        7995.1459960938f,
                        54.930736541748f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3319.087890625f,
                7472.4760742188f,
                55.027889251709f), new Vector3
                    (3388.0522460938f,
                        7101.2568359375f,
                        54.486026763916f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3989.9423828125f,
                7929.3422851563f,
                51.94282913208f), new Vector3
                    (3671.623046875f,
                        7723.146484375f,
                        53.906265258789f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4936.8452148438f,
                10547.737304688f,
                -63.064865112305f), new Vector3
                    (5156.7397460938f,
                        10853.216796875f,
                        52.951190948486f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5028.1235351563f,
                10115.602539063f,
                -63.082695007324f), new Vector3
                    (5423f,
                        10127,
                        55.15357208252f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6035.4819335938f,
                10973.666015625f,
                53.918266296387f), new Vector3
                    (6385.4013671875f,
                        10827.455078125f,
                        54.63500213623f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4747.0625f,
                11866.421875f,
                41.584358215332f), new Vector3
                    (4743.23046875f,
                        11505.842773438f,
                        51.196254730225f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6749.4487304688f,
                12980.83984375f,
                44.903495788574f), new Vector3
                    (6701.4965820313f,
                        12610.278320313f,
                        52.563804626465f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3114.1865234375f,
                9420.5078125f,
                -42.718975067139f), new Vector3
                    (2757f,
                        9255,
                        53.77322769165f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2786.8354492188f,
                9547.8935546875f,
                53.645294189453f), new Vector3
                    (3002.0930175781f,
                        9854.39453125f,
                        -53.198081970215f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3803.9470214844f,
                7197.9018554688f,
                53.730079650879f), new Vector3
                    (3664.1088867188f,
                        7543.572265625f,
                        54.18229675293f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2340.0886230469f,
                6387.072265625f,
                60.165466308594f), new Vector3
                    (2695.6096191406f,
                        6374.0634765625f,
                        54.339839935303f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3249.791015625f,
                6446.986328125f,
                55.605854034424f), new Vector3
                    (3157.4558105469f,
                        6791.4458007813f,
                        54.080295562744f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3823.6242675781f,
                5923.9130859375f,
                55.420352935791f), new Vector3
                    (3584.2561035156f,
                        6215.4931640625f,
                        55.6123046875f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5796.4809570313f,
                5060.4116210938f,
                51.673671722412f), new Vector3
                    (5730.3081054688f,
                        5430.1635742188f,
                        54.921173095703f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6007.3481445313f,
                4985.3803710938f,
                51.673641204834f), new Vector3
                    (6388.783203125f,
                        4987,
                        51.673400878906f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7040.9892578125f,
                3964.6728515625f,
                57.192108154297f), new Vector3
                    (6668.0073242188f,
                        3993.609375f,
                        51.671356201172f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7763.541015625f,
                3294.3481445313f,
                54.872283935547f), new Vector3
                    (7629.421875f,
                        3648.0581054688f,
                        56.908012390137f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4705.830078125f,
                9440.6572265625f,
                -62.586814880371f), new Vector3
                    (4779.9809570313f,
                        9809.9091796875f,
                        -63.09009552002f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4056.7907714844f,
                10216.12109375f,
                -63.152275085449f), new Vector3
                    (3680.1550292969f,
                        10182.296875f,
                        -63.701038360596f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4470.0883789063f,
                12000.479492188f,
                41.59789276123f), new Vector3
                    (4232.9799804688f,
                        11706.015625f,
                        49.295585632324f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5415.5708007813f,
                12640.216796875f,
                40.682685852051f), new Vector3
                    (5564.4409179688f,
                        12985.860351563f,
                        41.373748779297f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6053.779296875f,
                12567.381835938f,
                40.587882995605f), new Vector3
                    (6045.4555664063f,
                        12942.313476563f,
                        41.211364746094f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4454.66015625f,
                8057.1313476563f,
                42.799690246582f), new Vector3
                    (4577.8681640625f,
                        7699.3686523438f,
                        53.31339263916f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7754.7700195313f,
                10449.736328125f,
                52.890430450439f), new Vector3
                    (8096.2885742188f,
                        10288.80078125f,
                        53.66955947876f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7625.3139648438f,
                9465.7001953125f,
                55.008113861084f), new Vector3
                    (7995.986328125f,
                        9398.1982421875f,
                        53.530490875244f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(9767f,
                8839f,
                53.044532775879f), new Vector3
                    (9653.1220703125f,
                        9174.7626953125f,
                        53.697280883789f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10775.653320313f,
                7612.6943359375f,
                55.35241317749f), new Vector3
                    (10665.490234375f,
                        7956.310546875f,
                        65.222145080566f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10398.484375f,
                8257.8642578125f,
                66.200691223145f), new Vector3
                    (10176.104492188f,
                        8544.984375f,
                        64.849853515625f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11198.071289063f,
                8440.4638671875f,
                67.641044616699f), new Vector3
                    (11531.436523438f,
                        8611.0087890625f,
                        53.454048156738f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11686.700195313f,
                8055.9624023438f,
                55.458232879639f), new Vector3
                    (11314.19140625f,
                        8005.4946289063f,
                        58.438243865967f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10707.119140625f,
                7335.1752929688f,
                55.350387573242f), new Vector3
                    (10693f,
                        6943,
                        54.870254516602f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10395.380859375f,
                6938.5009765625f,
                54.869094848633f), new Vector3
                    (10454.955078125f,
                        7316.7041015625f,
                        55.308219909668f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10358.5859375f,
                6677.1704101563f,
                54.86909866333f), new Vector3
                    (10070.067382813f,
                        6434.0815429688f,
                        55.294486999512f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11161.98828125f,
                5070.447265625f,
                53.730766296387f), new Vector3
                    (10783f,
                        4965,
                        -63.57177734375f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11167.081054688f,
                4613.9829101563f,
                -62.898971557617f), new Vector3
                    (11501f,
                        4823,
                        54.571090698242f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11743.823242188f,
                4387.4672851563f,
                52.005855560303f), new Vector3
                    (11379f,
                        4239,
                        -61.565242767334f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10388.120117188f,
                4267.1796875f,
                -63.61775970459f), new Vector3
                    (10033.036132813f,
                        4147.1669921875f,
                        -60.332069396973f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8964.7607421875f,
                4214.3833007813f,
                -63.284225463867f), new Vector3
                    (8569f,
                        4241,
                        55.544258117676f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5554.8657226563f,
                4346.75390625f,
                51.680099487305f), new Vector3
                    (5414.0634765625f,
                        4695.6860351563f,
                        51.611679077148f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7311.3393554688f,
                10553.6015625f,
                54.153884887695f), new Vector3
                    (6938.5209960938f,
                        10535.8515625f,
                        54.441242218018f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7669.353515625f,
                5960.5717773438f,
                -64.488967895508f), new Vector3
                    (7441.2182617188f,
                        5761.8989257813f,
                        54.347793579102f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7949.65625f,
                2647.0490722656f,
                54.276401519775f), new Vector3
                    (7863.0063476563f,
                        3013.7814941406f,
                        55.178623199463f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8698.263671875f,
                3783.1169433594f,
                57.178703308105f), new Vector3
                    (9041f,
                        3975,
                        -63.242683410645f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(9063f,
                3401f,
                68.192077636719f), new Vector3
                    (9275.0751953125f,
                        3712.8935546875f,
                        -63.257461547852f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(12064.340820313f,
                6424.11328125f,
                54.830627441406f), new Vector3
                    (12267.9375f,
                        6742.9453125f,
                        54.83561706543f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(12797.838867188f,
                5814.9653320313f,
                58.281986236572f), new Vector3
                    (12422.740234375f,
                        5860.931640625f,
                        54.815074920654f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11913.165039063f,
                5373.34375f,
                54.050819396973f), new Vector3
                    (11569.1953125f,
                        5211.7143554688f,
                        57.787326812744f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(9237.3603515625f,
                2522.8937988281f,
                67.796775817871f), new Vector3
                    (9344.2041015625f,
                        2884.958984375f,
                        65.500213623047f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7324.2783203125f,
                1461.2199707031f,
                52.594970703125f), new Vector3
                    (7357.3852539063f,
                        1837.4309082031f,
                        54.282878875732f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6478.0454101563f,
                8342.501953125f,
                -64.045028686523f), new Vector3
                    (6751f,
                        8633,
                        56.019004821777f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6447f,
                8663f,
                56.018882751465f), new Vector3
                    (6413f,
                        8289,
                        62.786361694336f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6195.8334960938f,
                8559.810546875f,
                -65.304061889648f), new Vector3
                    (6327f,
                        56.517200469971f,
                        8913
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7095f,
                8763f,
                56.018997192383f), new Vector3
                    (7337f,
                        55.616943359375f,
                        9047
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7269f,
                9055f,
                55.611968994141f), new Vector3
                    (7027f,
                        8767,
                        56.018997192383f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5407f,
                10095f,
                55.045528411865f), new Vector3
                    (5033f,
                        10119,
                        63.082427978516f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5047f,
                10113f,
                -63.08129119873f), new Vector3
                    (5423f,
                        10109,
                        55.007797241211f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4747f,
                9463f,
                -62.445854187012f), new Vector3
                    (4743f,
                        9837,
                        -63.093593597412f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4769f,
                9677f,
                -63.086654663086f), new Vector3
                    (4775f,
                        9301,
                        -63.474864959717f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6731f,
                8089f,
                -64.655540466309f), new Vector3
                    (7095f,
                        8171,
                        56.051624298096f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7629.0434570313f,
                9462.6982421875f,
                55.042400360107f), new Vector3
                    (8019f,
                        9467,
                        53.530429840088f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7994.2685546875f,
                9477.142578125f,
                53.530174255371f), new Vector3
                    (7601f,
                        9441,
                        55.379856109619f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6147f,
                11063f,
                54.117427825928f), new Vector3
                    (6421f,
                        10805,
                        54.63500213623f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5952.1977539063f,
                11382.287109375f,
                54.240119934082f), new Vector3
                    (5889f,
                        11773,
                        39.546829223633f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6003.1401367188f,
                11827.516601563f,
                39.562377929688f), new Vector3
                    (6239f,
                        11479,
                        54.632926940918f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3947f,
                8013f,
                51.929698944092f), new Vector3
                    (3647f,
                        7789,
                        54.027297973633f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(1597f,
                8463f,
                54.923656463623f), new Vector3
                    (1223f,
                        8455,
                        50.640468597412f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(1247f,
                8413f,
                50.737510681152f), new Vector3
                    (1623f,
                        8387,
                        54.923782348633f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2440.49609375f,
                10038.1796875f,
                53.364398956299f), new Vector3
                    (2827f,
                        10205,
                        -64.97053527832f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2797f,
                10213f,
                -65.165946960449f), new Vector3
                    (2457f,
                        10055,
                        53.364398956299f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2797f,
                9563f,
                53.640556335449f), new Vector3
                    (3167f,
                        9625,
                        -63.810096740723f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3121.9699707031f,
                9574.16015625f,
                -63.448329925537f), new Vector3
                    (2755f,
                        9409,
                        53.722351074219f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3447f,
                7463f,
                55.021110534668f), new Vector3
                    (3581f,
                        7113,
                        54.248985290527f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3527f,
                7151f,
                54.452239990234f), new Vector3
                    (3372.861328125f,
                        7507.2211914063f,
                        55.13143157959f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2789f,
                6085f,
                55.241321563721f), new Vector3
                    (2445f,
                        5941,
                        60.189605712891f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2573f,
                5915f,
                60.192783355713f), new Vector3
                    (2911f,
                        6081,
                        55.503971099854f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3005f,
                5797f,
                55.631782531738f), new Vector3
                    (2715f,
                        5561,
                        60.190528869629f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2697f,
                5615f,
                60.190807342529f), new Vector3
                    (2943f,
                        5901,
                        55.629695892334f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3894.1960449219f,
                7192.3720703125f,
                53.4684715271f), new Vector3
                    (3641f,
                        7495,
                        54.714691162109f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3397f,
                6515f,
                55.605663299561f), new Vector3
                    (3363f,
                        6889,
                        53.412925720215f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3347f,
                6865f,
                53.312397003174f), new Vector3
                    (3343f,
                        6491,
                        55.605716705322f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3705f,
                7829f,
                53.67945098877f), new Vector3
                    (4009f,
                        8049,
                        51.996047973633f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7581f,
                5983f,
                -65.361351013184f), new Vector3
                    (7417f,
                        5647,
                        54.716590881348f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7495f,
                5753f,
                53.744125366211f), new Vector3
                    (7731f,
                        6045,
                        -64.48851776123f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7345f,
                6165f,
                -52.344753265381f), new Vector3
                    (7249f,
                        5803,
                        55.641929626465f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7665.0073242188f,
                5645.7431640625f,
                54.999004364014f), new Vector3
                    (7997f,
                        5861,
                        -62.778995513916f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7995f,
                5715f,
                -61.163398742676f), new Vector3
                    (7709f,
                        5473,
                        56.321662902832f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8653f,
                4441f,
                55.073780059814f), new Vector3
                    (9027f,
                        4425,
                        -61.594711303711f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8931f,
                4375f,
                -62.612571716309f), new Vector3
                    (8557f,
                        4401,
                        55.506855010986f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8645f,
                4115f,
                55.960289001465f), new Vector3
                    (9005f,
                        4215,
                        -63.280235290527f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8948.08203125f,
                4116.5078125f,
                -63.252712249756f), new Vector3
                    (8605f,
                        3953,
                        56.22159576416f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(9345f,
                2815f,
                67.37971496582f), new Vector3
                    (9375f,
                        2443,
                        67.509948730469f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(9355f,
                2537f,
                67.649841308594f), new Vector3
                    (9293f,
                        2909,
                        63.953853607178f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8027f,
                3029f,
                56.071315765381f), new Vector3
                    (8071f,
                        2657,
                        54.276405334473f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7995.0229492188f,
                2664.0703125f,
                54.276401519775f), new Vector3
                    (7985f,
                        3041,
                        55.659393310547f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5785f,
                5445f,
                54.918552398682f), new Vector3
                    (5899f,
                        5089,
                        51.673694610596f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5847f,
                5065f,
                51.673683166504f), new Vector3
                    (5683f,
                        5403,
                        54.923862457275f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6047f,
                4865f,
                51.67359161377f), new Vector3
                    (6409f,
                        4765,
                        51.673400878906f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6347f,
                4765f,
                51.673400878906f), new Vector3
                    (5983f,
                        4851,
                        51.673580169678f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6995f,
                5615f,
                55.738128662109f), new Vector3
                    (6701f,
                        5383,
                        61.461639404297f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6697f,
                5369f,
                61.083110809326f), new Vector3
                    (6889f,
                        5693,
                        55.628131866455f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11245f,
                4515f,
                52.104347229004f), new Vector3
                    (11585f,
                        4671,
                        52.104347229004f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11491.91015625f,
                4629.763671875f,
                52.506042480469f), new Vector3
                    (11143f,
                        4493,
                        -63.063579559326f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11395f,
                4315f,
                -62.597496032715f), new Vector3
                    (11579f,
                        4643,
                        51.962089538574f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11245f,
                4915f,
                53.017200469971f), new Vector3
                    (10869f,
                        4907,
                        -63.132637023926f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10923.66015625f,
                4853.9931640625f,
                -63.288948059082f), new Vector3
                    (11295f,
                        4913,
                        53.402942657471f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10595f,
                6965f,
                54.870422363281f), new Vector3
                    (10351f,
                        7249,
                        55.198459625244f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10415f,
                7277f,
                55.269580841064f), new Vector3
                    (10609f,
                        6957,
                        54.870502471924f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(12395f,
                6115f,
                54.809947967529f), new Vector3
                    (12759f,
                        6201,
                        57.640727996826f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(12745f,
                6265f,
                57.225738525391f), new Vector3
                    (12413f,
                        6089,
                        54.803039550781f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(12645f,
                4615f,
                53.343021392822f), new Vector3
                    (12349f,
                        4849,
                        56.222766876221f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(12395f,
                4765f,
                52.525123596191f), new Vector3
                    (12681f,
                        4525,
                        53.853294372559f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11918.497070313f,
                5471f,
                57.399909973145f), new Vector3
                    (11535f,
                        5471,
                        54.801097869873f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11593f,
                5501f,
                54.610706329346f), new Vector3
                    (11967f,
                        5477,
                        56.541202545166f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11140.984375f,
                8432.9384765625f,
                65.858421325684f), new Vector3
                    (11487f,
                        8625,
                        53.453464508057f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11420.7578125f,
                8608.6923828125f,
                53.453437805176f), new Vector3
                    (11107f,
                        8403,
                        65.090522766113f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11352.48046875f,
                8007.10546875f,
                57.916156768799f), new Vector3
                    (11701f,
                        8165,
                        55.458843231201f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11631f,
                8133f,
                55.45885848999f), new Vector3
                    (11287f,
                        7979,
                        58.037368774414f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10545f,
                7913f,
                65.745803833008f), new Vector3
                    (55.338600158691f,
                        10555f,
                        7537
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10795f,
                7613f,
                55.354972839355f), new Vector3
                    (10547f,
                        7893,
                        65.771072387695f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10729f,
                7307f,
                55.352409362793f), new Vector3
                    (10785f,
                        6937,
                        54.87170791626f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10745f,
                6965f,
                54.871494293213f), new Vector3
                    (10647f,
                        7327,
                        55.350120544434f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10099f,
                8443f,
                66.309921264648f), new Vector3
                    (10419f,
                        8249,
                        66.106910705566f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(9203f,
                3309f,
                63.777507781982f), new Vector3
                    (9359f,
                        3651,
                        -63.260040283203f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(9327f,
                3675f,
                -63.258842468262f), new Vector3
                    (9185f,
                        3329,
                        65.192367553711f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10045f,
                6465f,
                55.140678405762f), new Vector3
                    (10353f,
                        6679,
                        54.869094848633f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(10441f,
                8315.2333984375f,
                65.793014526367f), new Vector3
                    (10133f,
                        8529,
                        64.52165222168f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8323f,
                9137f,
                54.89501953125f), new Vector3
                    (8207f,
                        9493,
                        53.530456542969f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8295f,
                9363f,
                53.530418395996f), new Vector3
                    (8359f,
                        8993,
                        54.895038604736f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8495f,
                9763f,
                52.768348693848f), new Vector3
                    (8401f,
                        10125,
                        53.643203735352f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8419f,
                9997f,
                53.59920501709f), new Vector3
                    (8695f,
                        9743,
                        51.417175292969f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7145f,
                5965f,
                55.597702026367f), new Vector3
                    (7413f,
                        6229,
                        -66.513969421387f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6947f,
                8213f,
                56.01900100708f), new Vector3
                    (6621f,
                        8029,
                        -62.816535949707f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6397f,
                10813f,
                54.634998321533f), new Vector3
                    (6121f,
                        11065,
                        54.092365264893f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6247f,
                11513f,
                54.6325340271f), new Vector3
                    (6053f,
                        11833,
                        39.563938140869f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4627f,
                11897f,
                41.618049621582f), new Vector3
                    (4541f,
                        11531,
                        51.561706542969f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5179f,
                10839f,
                53.036727905273f), new Vector3
                    (4881f,
                        10611,
                        -63.11701965332f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4897f,
                10613f,
                -63.125648498535f), new Vector3
                    (5177f,
                        10863,
                        52.773872375488f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11367f,
                9751f,
                50.348838806152f), new Vector3
                    (11479f,
                        10107,
                        106.51720428467f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(11489f,
                10093f,
                106.53769683838f), new Vector3
                    (11403f,
                        9727,
                        50.349449157715f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(12175f,
                9991f,
                106.80973052979f), new Vector3
                    (12143f,
                        9617,
                        50.354927062988f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(12155f,
                9623f,
                50.354919433594f), new Vector3
                    (12123f,
                        9995,
                        106.81489562988f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(9397f,
                12037f,
                52.484146118164f), new Vector3
                    (9769f,
                        12077,
                        106.21959686279f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(9745f,
                12063f,
                106.2202835083f), new Vector3
                    (9373f,
                        12003,
                        52.484580993652f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(9345f,
                12813f,
                52.689178466797f), new Vector3
                    (9719f,
                        12805,
                        106.20919799805f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4171f,
                2839f,
                109.72004699707f), new Vector3
                    (4489f,
                        3041,
                        54.030017852783f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4473f,
                3009f,
                54.04020690918f), new Vector3
                    (4115f,
                        2901,
                        110.06342315674f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2669f,
                4281f,
                105.9382019043f), new Vector3
                    (2759f,
                        4647,
                        57.061370849609f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2761f,
                4653f,
                57.062965393066f), new Vector3
                    (2681f,
                        4287,
                        106.2310256958f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(1623f,
                4487f,
                108.56233215332f), new Vector3
                    (1573f,
                        4859,
                        56.13228225708f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(1573f,
                4845f,
                56.048126220703f), new Vector3
                    (1589f,
                        4471,
                        108.56234741211f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2355.4450683594f,
                6366.453125f,
                60.167724609375f), new Vector3
                    (2731f,
                        6355,
                        54.617771148682f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2669f,
                6363f,
                54.488224029541f), new Vector3
                    (2295f,
                        6371,
                        60.163955688477f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2068.5336914063f,
                8898.5322265625f,
                54.921718597412f), new Vector3
                    (2457f,
                        8967,
                        53.765918731689f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2447f,
                8913f,
                53.763805389404f), new Vector3
                    (2099f,
                        8775,
                        54.922241210938f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(1589f,
                9661f,
                49.631057739258f), new Vector3
                    (1297f,
                        9895,
                        38.928337097168f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(1347f,
                9813f,
                39.538192749023f), new Vector3
                    (1609f,
                        9543,
                        50.499561309814f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3997f,
                10213f,
                -63.152000427246f), new Vector3
                    (3627f,
                        10159,
                        -64.785446166992f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(3709f,
                10171f,
                -63.07014465332f), new Vector3
                    (4085f,
                        10175,
                        -63.139434814453f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(9695f,
                12813f,
                106.20919799805f), new Vector3
                    (9353f,
                        12965,
                        95.629013061523f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5647f,
                9563f,
                55.136940002441f), new Vector3
                    (5647f,
                        9187,
                        -65.224411010742f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(5895f,
                3389f,
                52.799312591553f), new Vector3
                    (6339f,
                        3633,
                        51.669734954834f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(6225f,
                3605f,
                51.669948577881f), new Vector3
                    (5793f,
                        3389,
                        53.080261230469f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8201f,
                1893f,
                54.276405334473f), new Vector3
                    (8333f,
                        1407,
                        52.60326385498f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8185f,
                1489f,
                52.59805679321f), new Vector3
                    (8015f,
                        1923,
                        54.276405334473f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2351f,
                4743f,
                56.366249084473f), new Vector3
                    (2355f,
                        4239,
                        107.71157836914f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(2293f,
                4389f,
                109.00361633301f), new Vector3
                    (2187f,
                        4883,
                        56.207984924316f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4271f,
                2065f,
                108.56426239014f), new Vector3
                    (4775f,
                        2033,
                        54.37939453125f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(4675f,
                2013f,
                54.971534729004f), new Vector3
                    (4173f,
                        1959,
                        108.41383361816f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(7769f,
                10925f,
                53.940235137939f), new Vector3
                    (8257f,
                        11049,
                        49.935401916504f
                    )
                ));
            Jumplist.Add(new Coordinate(new Vector3(8123f,
                11051f,
                49.935398101807f), new Vector3
                    (7689f,
                        10831,
                        53.834579467773f
                    )
                ));

        }
    }
}