using System;

namespace LifeLongApi.Codes
{
    public class AppHelper
    {
        public enum AppointmentStatus
        {
            ALL, //Just for querying the database alone.
            COMPLETED, //both parties were online at the agreed time and date.
            MISSED, //still not clear how this works.
            PENDING, //date of appointment hasn't arrived yet.
            POSTPONED //the same as pending, the difference is that it's been edited.
        }

        public enum FollowStatus
        {//this enum holds the different available status for the follow table.

            //confirmed means there's a mutual relationship between the mentor and the mentee
            CONFIRMED,
            //OnHold means the mentor is occupied with so many mentees at the moment and they will come back to review the mentorship request.
            ONHOLD,
            //Pending means the mentor hasn't taking any action on the request yet. The request hasn't been declined, accepted or put on hold.
            PENDING,
            //Declined means, well, request declined.
            DECLINED
        }

        public enum NotificationType
        {//name is self explanatory.
            APPOINTMENT,
            MENTORSHIPREQUEST
        }
    }
}