using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace PSEngagementSystem
{
    public static class MeetingRepository
    //repository for managing meetings in the SQLite database
    {
        //connection string for SQLite database
        public static string ConnectionString = "Data Source=ps_engagement.db";

        public static void InitializeDatabase()
        //ensures the Meetings table exists in the database
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS Meetings (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentID INTEGER NOT NULL,
                SupervisorID INTEGER NOT NULL,
                ScheduledTime TEXT NOT NULL,
                Status TEXT NOT NULL,
                RequestedBy TEXT NOT NULL
            );";
            command.ExecuteNonQuery();
        }
        //adds a new meeting to the database

        public static void AddMeeting(int studentId, int supervisorId, DateTime scheduledTime, string requestedBy)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"INSERT INTO Meetings (StudentID, SupervisorID, ScheduledTime, Status, RequestedBy)
              VALUES ($studentId, $supervisorId, $scheduledTime, $status, $requestedBy);";

            command.Parameters.AddWithValue("$studentId", studentId);
            command.Parameters.AddWithValue("$supervisorId", supervisorId);
            command.Parameters.AddWithValue("$scheduledTime", scheduledTime.ToString("yyyy-MM-dd HH:mm")); // stores datetime as string
            command.Parameters.AddWithValue("$status", MeetingStatus.Requested.ToString()); 
            command.Parameters.AddWithValue("$requestedBy", requestedBy);

            command.ExecuteNonQuery();
        }

        //updates the status of an existing meeting
        public static void UpdateMeetingStatus(int meetingId, MeetingStatus newStatus)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"UPDATE Meetings
              SET Status = $status
              WHERE ID = $meetingId;";

            command.Parameters.AddWithValue("$status", newStatus.ToString());
            command.Parameters.AddWithValue("$meetingId", meetingId);

            command.ExecuteNonQuery();
        }
        //retrieves meetings for a specific student
        public static List<Meeting> GetMeetingsByStudent(int studentId)
        {
            var meetings = new List<Meeting>();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                "SELECT ID, StudentID, SupervisorID, ScheduledTime, Status, RequestedBy FROM Meetings WHERE StudentID = $studentId;";
            command.Parameters.AddWithValue("$studentId", studentId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                meetings.Add(new Meeting
                {
                    ID = reader.GetInt32(0),
                    StudentID = reader.GetInt32(1),
                    SupervisorID = reader.GetInt32(2),
                    ScheduledTime = DateTime.Parse(reader.GetString(3)),
                    Status = Enum.Parse<MeetingStatus>(reader.GetString(4)),
                    RequestedBy = reader.GetString(5)
                });
            }

            return meetings;
        }

        //retrieves meetings for a specific supervisor
        public static List<Meeting> GetMeetingsBySupervisor(int supervisorId)
        {
            var meetings = new List<Meeting>();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                "SELECT ID, StudentID, SupervisorID, ScheduledTime, Status, RequestedBy FROM Meetings WHERE SupervisorID = $supervisorId;";
            command.Parameters.AddWithValue("$supervisorId", supervisorId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                meetings.Add(new Meeting
                {
                    ID = reader.GetInt32(0),
                    StudentID = reader.GetInt32(1),
                    SupervisorID = reader.GetInt32(2),
                    ScheduledTime = DateTime.Parse(reader.GetString(3)),
                    Status = Enum.Parse<MeetingStatus>(reader.GetString(4)),
                    RequestedBy = reader.GetString(5)
                });
            }

            return meetings;
        }
        //retrieves all meetings from the database
        public static List<Meeting> GetAllMeetings()
        {
            var meetings = new List<Meeting>();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                "SELECT ID, StudentID, SupervisorID, ScheduledTime, Status, RequestedBy FROM Meetings;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                meetings.Add(new Meeting
                {
                    ID = reader.GetInt32(0),
                    StudentID = reader.GetInt32(1),
                    SupervisorID = reader.GetInt32(2),
                    ScheduledTime = DateTime.Parse(reader.GetString(3)),
                    Status = Enum.Parse<MeetingStatus>(reader.GetString(4)),
                    RequestedBy = reader.GetString(5)
                });
            }

            return meetings;
        }
    }
}
