using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EmergencyResponseSim
{
    public class Incident
    {
        public string Type { get; }
        public string Location { get; }

        public Incident(string type, string location)
        {
            Type = type;
            Location = location;
        }

        public override string ToString()
        {
            return $"{Type} incident at {Location}";
        }
    }

    public abstract class EmergencyUnit
    {
        public string Name { get; protected set; }
        public int Speed { get; protected set; }

        protected EmergencyUnit(string name, int speed)
        {
            Name = name;
            Speed = speed;
        }

        // Check if this unit can deal with the given emergency type
        public abstract bool CanHandle(string incidentType);

        // How the unit responds when dispatched
        public abstract void RespondToIncident(Incident incident);
    }

    public class Police : EmergencyUnit
    {
        public Police() : base("Police Unit", 80) { }

        public override bool CanHandle(string incidentType)
        {
            return incidentType.Equals("Crime", StringComparison.OrdinalIgnoreCase);
        }

        public override void RespondToIncident(Incident incident)
        {
            Console.WriteLine($"  -> {Name} responding to {incident}. Securing the area.");
        }
    }

    public class Firefighter : EmergencyUnit
    {
        public Firefighter() : base("Fire Engine", 60) { }

        public override bool CanHandle(string incidentType)
        {
            return incidentType.Equals("Fire", StringComparison.OrdinalIgnoreCase);
        }

        public override void RespondToIncident(Incident incident)
        {
            Console.WriteLine($"  -> {Name} responding to {incident}. Extinguishing the fire.");
        }
    }

    public class Ambulance : EmergencyUnit
    {
        public Ambulance() : base("Ambulance", 70) { }

        public override bool CanHandle(string incidentType)
        {
            return incidentType.Equals("Medical", StringComparison.OrdinalIgnoreCase);
        }

        public override void RespondToIncident(Incident incident)
        {
            Console.WriteLine($"  -> {Name} responding to {incident}. Providing medical assistance.");
        }
    }

    class Program
    {
        private static readonly Random random = new Random();
        private static readonly string[] incidentTypes = { "Fire", "Crime", "Medical" };
        private static readonly string[] locations = { "Piassa", "Bole", "Lideta", "CMC", "Megenagna", "Addis Ababa University", "Meskel Square", "Sar Bet", "Old Airport", "Kirkos" };

        private static readonly List<EmergencyUnit> availableUnits = new List<EmergencyUnit>
        {
            new Police(),
            new Firefighter(),
            new Ambulance()
        };

        static void Main(string[] args)
        {
            int score = 0;
            int totalRounds = 5;

            Console.WriteLine("Emergency Response Simulation Starting");
            Console.WriteLine($"Available Units: {string.Join(", ", availableUnits.Select(u => u.Name))}");
            Console.WriteLine($"Simulation will run for {totalRounds} rounds.\n");

            for (int roundNum = 1; roundNum <= totalRounds; roundNum++)
            {
                Console.WriteLine($"--- Round {roundNum} ---");

                Console.WriteLine("Choose how to create incidents:");
                Console.WriteLine("  1. Generate 5 Random Incidents");
                Console.WriteLine("  2. Enter 1 Custom Incident");
                Console.Write("Your choice (1 or 2): ");
                string choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                List<Incident> incidentsThisRound = new List<Incident>();

                if (choice == "1")
                {
                    Console.WriteLine("Creating 5 random emergencies...");
                    for (int i = 0; i < 5; i++)
                    {
                        incidentsThisRound.Add(CreateRandomEmergency());
                    }
                }
                else if (choice == "2")
                {
                    Console.WriteLine("Let's create your custom emergency...");
                    incidentsThisRound.Add(CreateCustomEmergency());
                }
                else
                {
                    Console.WriteLine("Oops, that wasn't a valid choice. Creating 5 random emergencies instead...");
                    for (int i = 0; i < 5; i++)
                    {
                        incidentsThisRound.Add(CreateRandomEmergency());
                    }
                }

                Console.WriteLine($"Handling {incidentsThisRound.Count} emergency call(s) this round:");
                int incidentCounter = 0;

                foreach (Incident emergency in incidentsThisRound)
                {
                    incidentCounter++;
                    Console.WriteLine($"\nEmergency #{incidentCounter}: {emergency}");

                    EmergencyUnit unit = FindUnitForEmergency(emergency);

                    if (unit != null)
                    {
                        unit.RespondToIncident(emergency);
                        score += 10;
                        Console.WriteLine("  Good job! +10 points.");
                    }
                    else
                    {
                        Console.WriteLine($"  We don't have a unit that can handle {emergency.Type} emergencies!");
                        score -= 5;
                        Console.WriteLine("  Couldn't respond properly. -5 points.");
                    }

                    if (incidentsThisRound.Count > 1)
                    {
                        Thread.Sleep(500);
                    }
                }

                Console.WriteLine($"\nRound {roundNum} complete!");
                Console.WriteLine($"Current Score: {score}\n");
                Thread.Sleep(1000);
            }

            Console.WriteLine("Simulation Over");
            Console.WriteLine($"Your final score: {score}");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static EmergencyUnit FindUnitForEmergency(Incident emergency)
        {
            foreach (var unit in availableUnits)
            {
                if (unit.CanHandle(emergency.Type))
                {
                    return unit;
                }
            }
            return null;
        }

        private static Incident CreateRandomEmergency()
        {
            string randomType = incidentTypes[random.Next(incidentTypes.Length)];
            string randomLocation = locations[random.Next(locations.Length)];
            return new Incident(randomType, randomLocation);
        }

        private static Incident CreateCustomEmergency()
        {
            string emergencyType = "";
            bool validType = false;

            while (!validType)
            {
                Console.Write($"What kind of emergency? ({string.Join(", ", incidentTypes)}): ");
                emergencyType = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(emergencyType) &&
                    incidentTypes.Contains(emergencyType, StringComparer.OrdinalIgnoreCase))
                {
                    emergencyType = char.ToUpper(emergencyType[0]) + emergencyType.Substring(1).ToLower();
                    validType = true;
                }
                else
                {
                    Console.WriteLine("That's not a valid emergency type. Try 'Fire', 'Crime', or 'Medical'.");
                }
            }

            Console.Write("Where is it happening? ");
            string location = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(location))
            {
                location = "Unknown Location";
                Console.WriteLine("Location not provided, using 'Unknown Location'.");
            }

            return new Incident(emergencyType, location);
        }
    }
}
