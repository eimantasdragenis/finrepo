namespace PSEngagementSystem
{
    class Program
    {
        // Hardcoded students and supervisors for simplicity and testing. 
        static List<Student> students = new List<Student>
        {
            new Student { ID = 1, Name = "Eddie", SupervisorID = 1 },
            new Student { ID = 2, Name = "Dylan", SupervisorID = 1 },
            new Student { ID = 3, Name = "Elisabeth", SupervisorID = 2 },
            new Student { ID = 4, Name = "Testing", SupervisorID = 2 }
        };

        static List<PersonalSupervisor> supervisors = new List<PersonalSupervisor>
        {
            new PersonalSupervisor { ID = 1, Name = "Dr John Whelan", StudentIDs = new List<int> {1, 2} },
            new PersonalSupervisor { ID = 2, Name = "Mr.BoB John", StudentIDs = new List<int> {3} }
        };

        static void Main()
        {
            // Initialize databases
            CheckInRepository.InitializeDatabase();
            MeetingRepository.InitializeDatabase();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Personal Supervisor Engagement System");
                Console.WriteLine("1. Student");
                Console.WriteLine("2. Personal Supervisor");
                Console.WriteLine("3. Senior Tutor");
                Console.WriteLine("4. Exit");

                Console.Write("Select role: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                    StudentMenu();
                else if (choice == "2")
                    SupervisorMenu();
                else if (choice == "3")
                    SeniorTutorMenu();
                else if (choice == "4")
                    break;
            }
        }

        // STUDENT MENU.
        //DISPLAYS STUDENT MENU AND VALIDATES THE PROVIDED STUDENT ID.
        static void StudentMenu()
        {
            Console.Clear();
            Console.Write("Enter your Student ID: ");
            int studentId;
            while (!int.TryParse(Console.ReadLine(), out studentId) || !students.Any(s => s.ID == studentId))
            {
                Console.Write("Please enter a valid Student ID: ");
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Student Menu");
                Console.WriteLine("1. Log check-in");
                Console.WriteLine("2. View my check-ins");
                Console.WriteLine("3. Request a meeting with supervisor");
                Console.WriteLine("4. Exit");

                Console.Write("Select option: ");
                string option = Console.ReadLine();

                if (option == "1")
                    LogCheckIn(studentId);
                else if (option == "2")
                    ViewStudentCheckIns(studentId);
                else if (option == "3")
                    RequestMeeting(studentId);
                else if (option == "4")
                    break;
            }
        }
        // ASKS THE USER FOR THEIR MOOD. LOGS THE CHECK-IN WITH OPTIONAL COMMENTS.
        static void LogCheckIn(int studentId)
        {
            Console.Write("How are you feeling today? (1 = very poor, 5 = excellent): ");
            int mood;
            while (!int.TryParse(Console.ReadLine(), out mood) || mood < 1 || mood > 5)
            {
                Console.Write("Please enter a number between 1 and 5: ");
            }

            Console.Write(" Please enter any comments that you want to pass on to your supervisor (Press Enter skip comments): ");
            string comment = Console.ReadLine();
            try
            {
                CheckInRepository.AddCheckIn(studentId, mood, comment);

                // CONFIRMATION MESSAGE
                Console.WriteLine("Check-in saved !\n");
            }
            catch (InvalidOperationException ex)
            {
                // THROWS AN ERROR IF A CHECK-IN HAS ALREADY BEEN MADE TODAY.
                Console.WriteLine($"\n ERROR {ex.Message}");
                Console.WriteLine("You can only submit ONE mood check-in per day.\n");
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
        
        // RETRIEVES AND DISPLAYS THE CHECK-INS FOR THE LOGGED-IN STUDENT.
        static void ViewStudentCheckIns(int studentId)
        {
            Console.Clear();
            var allCheckIns = CheckInRepository.GetCheckIns();
            var myCheckIns = allCheckIns.Where(c => c.StudentID == studentId).ToList();

            Console.WriteLine("Your Check-Ins:");
            if (myCheckIns.Count == 0)
            {
                Console.WriteLine("No check-ins found.");
            }
            else
            {
                foreach (var c in myCheckIns)
                {
                    string commentText = string.IsNullOrWhiteSpace(c.Comment)
                        ? ""
                        : $" | Comment: {c.Comment}";

                    Console.WriteLine($"{c.Date} - Mood: {c.Mood}{commentText}");
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        //CREATES A MEETING REQUEST FOR THE STUDENT'S SUPERVISOR.
        static void RequestMeeting(int studentId)
        {
            Console.Clear();
            var student = students.Find(s => s.ID == studentId);
            int supervisorId = student.SupervisorID;

            Console.Write("Enter meeting date/time (yyyy-MM-dd HH:mm): ");
            DateTime dt;
            while (!DateTime.TryParse(Console.ReadLine(), out dt))
            {
                Console.Write("Please enter a valid date/time (yyyy-MM-dd HH:mm): ");
            }

            MeetingRepository.AddMeeting(studentId, supervisorId, dt, "Student");
            Console.WriteLine("Meeting requested successfully!\n");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        //  SUPERVISOR MENU 
        static void SupervisorMenu()
        {
            Console.Clear();
            Console.Write("Enter Supervisor ID: ");
            int supervisorId;
            while (!int.TryParse(Console.ReadLine(), out supervisorId) || !supervisors.Any(s => s.ID == supervisorId))
            {
                Console.Write("Please enter a valid Supervisor ID: ");
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Supervisor Menu");
                Console.WriteLine("1. View my students' check-ins");
                Console.WriteLine("2. View my meetings");
                Console.WriteLine("3. Schedule a meeting with a student");
                Console.WriteLine("4. Update Meeting Status");
                Console.WriteLine("5. Exit");

                Console.Write("Select option: ");
                string option = Console.ReadLine();

                if (option == "1")
                    ViewSupervisorCheckIns(supervisorId);
                else if (option == "2")
                    ViewSupervisorMeetings(supervisorId);
                else if (option == "3")
                    ScheduleMeeting(supervisorId);
                else if (option == "4")
                    UpdateMeetingStatusMenu(supervisorId);
                else if (option == "5")
                    break;
            }
        }

        //DISPLAYS ALL THE CHECK-INS FOR THE STUDENTS ASSIGNED TO THE LOGGED-IN SUPERVISOR.
        static void ViewSupervisorCheckIns(int supervisorId)
        {
            Console.Clear();
            var myStudents = students.Where(s => s.SupervisorID == supervisorId).ToList();
            var checkIns = CheckInRepository.GetCheckIns();

            Console.WriteLine("Students' Check-Ins:");
            bool found = false;

            foreach (var s in myStudents)
            {
                foreach (var c in checkIns.Where(c => c.StudentID == s.ID))
                {
                    string commentText = string.IsNullOrWhiteSpace(c.Comment)
                        ? ""
                        : $" | Comment: {c.Comment}";

                    Console.WriteLine($"{s.Name} - {c.Date} - Mood: {c.Mood}{commentText}");
                    found = true;
                }
            }

            if (!found)
                Console.WriteLine("No check-ins found.");

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        //DISPLAYS ALL MEETINGS FOR THE LOGGED-IN SUPERVISOR.
        static void ViewSupervisorMeetings(int supervisorId)
        {
            Console.Clear();
            var meetings = MeetingRepository.GetMeetingsBySupervisor(supervisorId);

            Console.WriteLine("Your Meetings:");
            if (meetings.Count == 0)
                Console.WriteLine("No meetings scheduled.");
            else
            {
                foreach (var m in meetings)
                    Console.WriteLine($"{m.ScheduledTime} - StudentID: {m.StudentID} - Status: {m.Status} - RequestedBy: {m.RequestedBy}");
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        // ALLOWS THE SUPERVISOR TO UPDATE THE STATUS OF A MEETING.
        static void UpdateMeetingStatusMenu(int supervisorId)
        {
            Console.Clear();

            // Show supervisor's meetings first
            var meetings = MeetingRepository.GetMeetingsBySupervisor(supervisorId);

            Console.WriteLine("Your Meetings:");
            if (meetings.Count == 0)
            {
                Console.WriteLine("No meetings found.");
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
                return;
            }
            // LIST OF MEETINGS WITH DETAILS
            foreach (var m in meetings)
            {
                Console.WriteLine($"ID: {m.ID} | {m.ScheduledTime} | StudentID: {m.StudentID} | Status: {m.Status} | RequestedBy: {m.RequestedBy}");
            }

            Console.Write("\nEnter Meeting ID to update: ");
            int meetingId;
            while (!int.TryParse(Console.ReadLine(), out meetingId) || !meetings.Any(m => m.ID == meetingId))
            {
                Console.Write("Please enter a valid Meeting ID from the list above: ");
            }

            Console.WriteLine("\nSelect new status:");
            Console.WriteLine("1. Confirmed");
            Console.WriteLine("2. Cancelled");
            Console.WriteLine("3. Completed");
            Console.Write("Choice: ");

            string choice = Console.ReadLine();
            MeetingStatus newStatus;

            if (choice == "1")
                newStatus = MeetingStatus.Confirmed;
            else if (choice == "2")
                newStatus = MeetingStatus.Cancelled;
            else if (choice == "3")
                newStatus = MeetingStatus.Completed;
            else
            {
                Console.WriteLine("Invalid choice.");
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
                return;
            }

            MeetingRepository.UpdateMeetingStatus(meetingId, newStatus);

            Console.WriteLine($"\nMeeting {meetingId} updated to {newStatus} successfully!");
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        static void ScheduleMeeting(int supervisorId)
        {
            Console.Clear();
            Console.Write("Enter Student ID: ");
            int studentId;
            while (!int.TryParse(Console.ReadLine(), out studentId) || !students.Any(s => s.ID == studentId))
            {
                Console.Write("Please enter a valid Student ID: ");
            }

            Console.Write("Enter meeting date/time (yyyy-MM-dd HH:mm): ");
            DateTime dt;
            while (!DateTime.TryParse(Console.ReadLine(), out dt))
            {
                Console.Write("Please enter a valid date/time: ");
            }

            MeetingRepository.AddMeeting(studentId, supervisorId, dt, "Supervisor");
            Console.WriteLine("Meeting scheduled successfully!\n");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        // SENIOR TUTOR MENU
        static void SeniorTutorMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Senior Tutor Menu");
                Console.WriteLine("1. View all students");
                Console.WriteLine("2. View all check-ins");
                Console.WriteLine("3. View all meetings");
                Console.WriteLine("4. Exit");

                Console.Write("Select option: ");
                string option = Console.ReadLine();

                if (option == "1")
                {
                    Console.WriteLine("\nStudents:");
                    foreach (var s in students)
                    //DISPLAYS STUDENT DETAILS
                    {
                        Console.WriteLine($"ID: {s.ID}, Name: {s.Name}, SupervisorID: {s.SupervisorID}");
                    }

                    Console.WriteLine("\nPress Enter to return to the menu...");
                    Console.ReadLine();
                }
                else if (option == "2")
                {
                    var checkIns = CheckInRepository.GetCheckIns();
                    Console.WriteLine("\nAll Check-Ins:");

                    if (checkIns.Count == 0)
                    {
                        Console.WriteLine("No check-ins found.");
                    }
                    else
                    {
                        foreach (var c in checkIns)
                        {
                            var student = students.Find(s => s.ID == c.StudentID);
                            if (student != null)
                                Console.WriteLine($"{student.Name} - {c.Date} - Mood: {c.Mood}");
                            else
                                Console.WriteLine($"Unknown student ID {c.StudentID} - {c.Date} - Mood: {c.Mood}");
                        }
                    }

                    Console.WriteLine("\nPress Enter to return to the menu...");
                    Console.ReadLine();
                }
                else if (option == "3")
                {
                    var meetings = MeetingRepository.GetAllMeetings();
                    Console.WriteLine("\nAll Meetings:");

                    if (meetings.Count == 0)
                    {
                        Console.WriteLine("No meetings scheduled.");
                    }
                    else
                    {
                        foreach (var m in meetings)
                        {
                            var student = students.Find(s => s.ID == m.StudentID);
                            var supervisor = supervisors.Find(s => s.ID == m.SupervisorID);

                            string studentName = student != null ? student.Name : $"Unknown student ID {m.StudentID}";
                            string supervisorName = supervisor != null ? supervisor.Name : $"Unknown supervisor ID {m.SupervisorID}";

                            Console.WriteLine($"{m.ScheduledTime} - Student: {studentName}, Supervisor: {supervisorName}, Status: {m.Status}, RequestedBy: {m.RequestedBy}");
                        }
                    }

                    Console.WriteLine("\nPress Enter to return to the menu...");
                    Console.ReadLine();
                }
                else if (option == "4")
                {
                    break;
                }
            }
        }
    }
}
