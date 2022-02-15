namespace Tasks.Runtime.Domain.Constants
{
    public enum ErrorCodes
    {
        TaskDoesntExist,
        TaskFinnishedOrCancelled,
        TaskAlreadyStarted,
        TaskAlreadyInStandby,
        TaskHasNoAllocatedUser
    }
}
