using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EliteDangerousDataFeed
{
    public class StatusResult
    {
        [JsonProperty(PropertyName = "event")]
        public string Event { get; set; }

        [JsonProperty(PropertyName = "currentStatus")]
        public Dictionary<string, bool> CurrentStatus { get; set; }
    }

    public class StatusGenerator
    {
        public Dictionary<string, bool> GenerateStatus(int statusFlags)
        {
            BitArray b = new BitArray(new int[] { statusFlags });
            bool[] flags = b.Cast<bool>().ToArray();
            var result = new Dictionary<string, bool>();
            result.Add("Docked", flags[0]);
            result.Add("Landed", flags[1]);
            result.Add("LandingGearDown", flags[2]);
            result.Add("ShieldsUp", flags[3]);
            result.Add("Supercruise", flags[4]);
            result.Add("FlightAssistOff", flags[5]);
            result.Add("HardpointsDeployed", flags[6]);
            result.Add("InWing", flags[7]);
            result.Add("LightsOn", flags[8]);
            result.Add("CargoScoopDeployed", flags[9]);
            result.Add("SilentRunning", flags[10]);
            result.Add("ScoopingFuel", flags[11]);
            result.Add("SrvHandbrake", flags[12]);
            result.Add("SrvTurret", flags[13]);
            result.Add("SrvUnderShip", flags[14]);
            result.Add("SrvDriveAssist", flags[15]);
            result.Add("FsdMassLocked", flags[16]);
            result.Add("FsdCharging", flags[17]);
            result.Add("FsdCooldown", flags[18]);
            result.Add("LowFuel", flags[19]);
            result.Add("OverHeating", flags[20]);
            result.Add("HasLatLong", flags[21]);
            result.Add("IsInDanger", flags[22]);
            result.Add("BeingInterdicted", flags[23]);

            return result;
        }
    }

        public class Status
        {
            public bool Docked { get; set; }
            public bool Landed { get; set; }
            public bool LandingGearDown { get; set; }
            public bool ShieldsUp { get; set; }
            public bool Supercruise { get; set; }
            public bool FlightAssistOff { get; set; }
            public bool HardpointsDeployed { get; set; }
            public bool InWing { get; set; }
            public bool LightsOn { get; set; }
            public bool CargoScoopDeployed { get; set; }
            public bool SilentRunning { get; set; }
            public bool ScoopingFuel { get; set; }
            public bool SrvHandbrake { get; set; }
            public bool SrvTurret { get; set; }
            public bool SrvUnderShip { get; set; }
            public bool SrvDriveAssist { get; set; }
            public bool FsdMassLocked { get; set; }
            public bool FsdCharging { get; set; }
            public bool FsdCooldown { get; set; }
            public bool LowFuel { get; set; }
            public bool OverHeating { get; set; }
            public bool HasLatLong { get; set; }
            public bool IsInDanger { get; set; }
            public bool BeingInterdicted { get; set; }
        }
}


/*Flags:
Bit Value Hex Meaning
0 1 0000 0001 Docked, (on a landing pad)
1 2 0000 0002 Landed, (on planet surface)
2 4 0000 0004 Landing Gear Down
3 8 0000 0008 Shields Up
4 16 0000 0010 Supercruise
5 32 0000 0020 FlightAssist Off
6 64 0000 0040 Hardpoints Deployed
7 128 0000 0080 In Wing
8 256 0000 0100 LightsOn
9 512 0000 0200 Cargo Scoop Deployed
10 1024 0000 0400 Silent Running,
11 2048 0000 0800 Scooping Fuel
12 4096 0000 1000 Srv Handbrake
13 8192 0000 2000 Srv Turret
14 16384 0000 4000 Srv UnderShip
15 32768 0000 8000 Srv DriveAssist
16 65536 0001 0000 Fsd MassLocked
17 131072 0002 0000 Fsd Charging
18 262144 0004 0000 Fsd Cooldown
19 524288 0008 0000 Low Fuel ( < 25% )
20 1048576 0010 0000 Over Heating ( > 100% )
21 2097152 0020 0000 Has Lat Long
22 4194304 0040 0000 IsInDanger
23 8388608 0080 0000 Being Interdicted
24 16777216 0100 0000 In MainShip
25 33554432 0200 0000 In Fighter
26 67108864 0400 0000 In SRV 
*/