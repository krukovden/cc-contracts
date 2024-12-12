using Abs.CommonCore.Contracts.Proto.Cloud.Port;
using Abs.CommonCore.Contracts.Proto.Cloud.Shared;
using Abs.CommonCore.Contracts.Proto.Cloud.VoyageManager;
using Abs.CommonCore.Contracts.Proto.Cloud.VoyagePlan;
using Abs.CommonCore.Contracts.Proto.Cloud.VoyagePlan.Veson;
using Abs.CommonCore.Contracts.Proto.Cloud.VoyagePlanResponse;
using Abs.CommonCore.Contracts.Proto.Cloud.Waypoints;
using Abs.CommonCore.Contracts.Proto.Cloud.Waypoints.Ecdis;
using Abs.CommonCore.Contracts.Proto.Drex.FileShipping;
using Abs.CommonCore.Contracts.Proto.Scheduler;
using Abs.CommonCore.Contracts.Proto.Tenant;
using Google.Protobuf;

namespace Abs.CommonCore.Contracts.Extensions;

public static class MessageVersionProvider
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Unreadable result.")]
    public static string GetVersion(Type type)
    {
        var fullName = type.FullName;
        if (fullName is null || type.GetInterface(nameof(IMessage)) is null)
        {
            throw new ArgumentOutOfRangeException($"Type '{fullName}' is not supported.");
        }

        // Shared
        if (fullName.Equals(typeof(Paging).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(Vessel).FullName))
        {
            return "1.0.0.0";
        }

        // Ports
        if (fullName.Equals(typeof(VesonPortResponse).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(UneceCountry).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(UnecePortResponse).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(UneceStatus).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(UneceFunction).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(UnecePort).FullName))
        {
            return "1.0.0.0";
        }

        // Waypoints
        if (fullName.Equals(typeof(VoyagePlanWaypoints).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(VoyagePlanWaypointsList).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(VoyagePlanWaypointsResponse).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(GeometryType).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(WaypointsFileStatus).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(ErrorReport).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(Error).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(ProcessingStatus).FullName))
        {
            return "1.0.0.0";
        }

        // Voyage plan
        if (fullName.Equals(typeof(VoyagePlan).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(PartnerVoyagePlan).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(PartnerSpecificVoyagePlanData).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(VoyagePlanItinerary).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(VoyagePlanStatus).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(VoyagePlanItineraryBunker).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(VoyagePlanItineraryBerth).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(VoyagePlanResponse).FullName))
        {
            return "1.0.0.0";
        }

        // Veson
        if (fullName.Equals(typeof(VesonSpecificData).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(VesonItinerary).FullName))
        {
            return "1.0.0.0";

        }

        if (fullName.Equals(typeof(VesonVoyagePlanCargoHandling).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(PrevVoyagePlanItinerary).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(VesonVoyagePlanTenantRequest).FullName))
        {
            return "1.0.0.0";
        }

        // Tenant
        if (fullName.Equals(typeof(Tenant).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(ClientInfo).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(TenantResponse).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(CustomPort).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(CreateCustomPortRequest).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(VesonInformation).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(EcdisInformation).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(ERPInformation).FullName))
        {
            return "1.0.0.0";
        }

        // Voyage manager
        if (fullName.Equals(typeof(Registry).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(RegistrationRequest).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(UnregistrationRequest).FullName))
        {
            return "1.0.0.0";
        }

        // Scheduler
        if (fullName.Equals(typeof(ScheduleRequest).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(ScheduleResponse).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(CreateScheduleRequest).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(DeleteScheduleRequest).FullName))
        {
            return "1.0.0.0";
        }

        // Drex
        if (fullName.Equals(typeof(DrexFileNotification).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(DrexFileResponse).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(DrexFileRequest).FullName))
        {
            return "1.0.0.0";
        }

        if (fullName.Equals(typeof(Proto.Shared.Application).FullName))
        {
            return "1.0.0.0";
        }

        throw new ArgumentOutOfRangeException($"Type '{fullName}' is not supported.");
    }
}
