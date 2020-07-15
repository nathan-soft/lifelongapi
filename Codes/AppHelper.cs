using System;

namespace LifeLongApi.Codes
{
    public class AppHelper
    {
        public enum FollowStatus
        {//this class holds the different available status for the follow table.

            //confirmed means there's a mutual relationship between the mentor and the mentee
            CONFIRMED,
            //OnHold means the mentor is occupied with so many mentees at the moment and they will come back to review the mentorship request.
            ONHOLD,
            //Pending means the mentor hasn't taking any action on the request yet. The request hasn't been declined, accepted or put on hold.
            PENDING
        }
    }
}