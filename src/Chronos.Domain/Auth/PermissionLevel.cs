namespace Chronos.Domain.Auth;

public enum PermissionLevel
{
    // TODO Will be used in the future
    /* Owner - one per organization
     * Admin - multiple per organization, lowest that has access to billing information
     * Operator - multiple per organization & managed schedule, manages schedules
     * Participant - participates in schedules, can post requests to schedules that is participating in
     * Reader - lowest, can only view schedules that has access to
     *
     * These roles would most likely be managed at the organization level, then per schedule.
     */
    Reader, Participant, Operator, Admin, Owner
}