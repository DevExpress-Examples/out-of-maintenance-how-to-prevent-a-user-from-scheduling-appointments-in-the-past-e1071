#region Using
using System;
using System.Web.UI.WebControls;
using DevExpress.XtraScheduler;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.ASPxScheduler.Internal;
#endregion

public partial class _Default : System.Web.UI.Page {
    ASPxSchedulerStorage Storage { get { return ASPxScheduler1.Storage; } }

    protected void Page_Load(object sender, EventArgs e) {
        SetupMappings();
        DemoUtils.FillResources(Storage, 5);

        ASPxScheduler1.AppointmentDataSource = appointmentDataSource;
        ASPxScheduler1.DataBind();
    }

    void SetupMappings() {
        ASPxAppointmentMappingInfo mappings = Storage.Appointments.Mappings;
        Storage.BeginUpdate();
        try {
            mappings.AppointmentId = "Id";
            mappings.Start = "StartTime";
            mappings.End = "EndTime";
            mappings.Subject = "Subject";
            mappings.AllDay = "AllDay";
            mappings.Description = "Description";
            mappings.Label = "Label";
            mappings.Location = "Location";
            mappings.RecurrenceInfo = "RecurrenceInfo";
            mappings.ReminderInfo = "ReminderInfo";
            mappings.ResourceId = "OwnerId";
            mappings.Status = "Status";
            mappings.Type = "EventType";
        }
        finally {
            Storage.EndUpdate();
        }
    }

    CustomEventList GetCustomEvents() {
        CustomEventList events = Session["ListBoundModeObjects"] as CustomEventList;
        if (events == null) {
            events = GenerateCustomEventList();
            Session["ListBoundModeObjects"] = events;
        }
        return events;
    }

    #region Random events generation
    CustomEventList GenerateCustomEventList() {
        CustomEventList eventList = new CustomEventList();
        int count = Storage.Resources.Count;
        for (int i = 0; i < count; i++) {
            Resource resource = Storage.Resources[i];
            string subjPrefix = resource.Caption + "'s ";

            eventList.Add(CreateEvent(resource.Id, subjPrefix + "meeting", 2, 5));
            eventList.Add(CreateEvent(resource.Id, subjPrefix + "travel", 3, 6));
            eventList.Add(CreateEvent(resource.Id, subjPrefix + "phone call", 0, 10));
        }
        return eventList;
    }
    CustomEvent CreateEvent(object resourceId, string subject, int status, int label) {
        CustomEvent customEvent = new CustomEvent();
        customEvent.Subject = subject;
        customEvent.OwnerId = resourceId;
        Random rnd = DemoUtils.RandomInstance;
        int rangeInHours = 48;
        customEvent.StartTime = DateTime.Today + TimeSpan.FromHours(rnd.Next(0, rangeInHours));
        customEvent.EndTime = customEvent.StartTime + TimeSpan.FromHours(rnd.Next(0, rangeInHours / 8));
        customEvent.Status = status;
        customEvent.Label = label;
        customEvent.Id = "ev" + customEvent.GetHashCode();
        return customEvent;
    }
    #endregion

    // User generated appointment id
    protected void ASPxScheduler1_AppointmentInserting(object sender, PersistentObjectCancelEventArgs e) {
        ASPxSchedulerStorage storage = (ASPxSchedulerStorage)sender;
        Appointment apt = (Appointment)e.Object;
        storage.SetAppointmentId(apt, "a" + apt.GetHashCode());
    }
    // Populating ObjectDataSource
    protected void appointmentsDataSource_ObjectCreated(object sender, ObjectDataSourceEventArgs e) {
        e.ObjectInstance = new CustomEventDataSource(GetCustomEvents());
    }

    protected void ASPxScheduler1_AllowAppointmentCreate(object sender, AppointmentOperationEventArgs e) {
        e.Allow = ASPxScheduler1.SelectedInterval.Start >= DateTime.Today;
    }

    protected void ASPxScheduler1_VisibleIntervalChanged(object sender, EventArgs e) {
        ASPxScheduler1.ApplyChanges(ASPxSchedulerChangeAction.RenderViewMenu);
    }

    protected void ASPxScheduler1_AllowAppointmentDrag(object sender, AppointmentOperationEventArgs e) {
        e.Allow = e.Appointment.Start >= DateTime.Today;
    }

    protected void ASPxScheduler1_PreparePopupMenu(object sender, PreparePopupMenuEventArgs e) {

    }


    protected void ASPxScheduler1_BeforeExecuteCallbackCommand(object sender, SchedulerCallbackCommandEventArgs e) {
        if (e.CommandId == "RecreateMenu") {
            e.Command = new MyCallbackCommand(ASPxScheduler1);
        }
    }

    public class MyCallbackCommand : SchedulerCallbackCommand {

        ASPxScheduler scheduler = null;

        public MyCallbackCommand(ASPxScheduler scheduler)
            : base(scheduler) {
            this.scheduler = scheduler;
        }

        public override string Id {
            get {
                return "RecreateMenu";
            }
        }

        protected override void ExecuteCore() {
            scheduler.ApplyChanges(ASPxSchedulerChangeAction.RenderAppointmentMenu);
        }

        protected override void ParseParameters(string parameters) {

        }
    }

    public class DemoUtils {
        public static Random RandomInstance = new Random();
        public static string[] Users = new string[] { "Peter Dolan", "Ryan Fischer", "Andrew Miller", "Tom Hamlett",
												"Jerry Campbell", "Carl Lucas", "Mark Hamilton", "Steve Lee" };

        public static void FillResources(ASPxSchedulerStorage storage, int count) {
            ResourceCollection resources = storage.Resources.Items;
            storage.BeginUpdate();
            try {
                int cnt = Math.Min(count, Users.Length);
                for (int i = 1; i <= cnt; i++)
                    resources.Add(new Resource(i, Users[i - 1]));
            }
            finally {
                storage.EndUpdate();
            }
        }
    }
}
