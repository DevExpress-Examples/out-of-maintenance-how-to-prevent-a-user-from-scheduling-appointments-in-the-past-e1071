#Region "Using"
Imports System
Imports System.Web.UI.WebControls
Imports DevExpress.XtraScheduler
Imports DevExpress.Web.ASPxScheduler
Imports DevExpress.Web.ASPxScheduler.Internal
#End Region

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Private ReadOnly Property Storage() As ASPxSchedulerStorage
        Get
            Return ASPxScheduler1.Storage
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        SetupMappings()
        DemoUtils.FillResources(Storage, 5)

        ASPxScheduler1.AppointmentDataSource = appointmentDataSource
        ASPxScheduler1.DataBind()
    End Sub

    Private Sub SetupMappings()
        Dim mappings As ASPxAppointmentMappingInfo = Storage.Appointments.Mappings
        Storage.BeginUpdate()
        Try
            mappings.AppointmentId = "Id"
            mappings.Start = "StartTime"
            mappings.End = "EndTime"
            mappings.Subject = "Subject"
            mappings.AllDay = "AllDay"
            mappings.Description = "Description"
            mappings.Label = "Label"
            mappings.Location = "Location"
            mappings.RecurrenceInfo = "RecurrenceInfo"
            mappings.ReminderInfo = "ReminderInfo"
            mappings.ResourceId = "OwnerId"
            mappings.Status = "Status"
            mappings.Type = "EventType"
        Finally
            Storage.EndUpdate()
        End Try
    End Sub

    Private Function GetCustomEvents() As CustomEventList

        Dim events_Renamed As CustomEventList = TryCast(Session("ListBoundModeObjects"), CustomEventList)
        If events_Renamed Is Nothing Then
            events_Renamed = GenerateCustomEventList()
            Session("ListBoundModeObjects") = events_Renamed
        End If
        Return events_Renamed
    End Function

    #Region "Random events generation"
    Private Function GenerateCustomEventList() As CustomEventList
        Dim eventList As New CustomEventList()
        Dim count As Integer = Storage.Resources.Count
        For i As Integer = 0 To count - 1
            Dim resource As Resource = Storage.Resources(i)
            Dim subjPrefix As String = resource.Caption & "'s "

            eventList.Add(CreateEvent(resource.Id, subjPrefix & "meeting", 2, 5))
            eventList.Add(CreateEvent(resource.Id, subjPrefix & "travel", 3, 6))
            eventList.Add(CreateEvent(resource.Id, subjPrefix & "phone call", 0, 10))
        Next i
        Return eventList
    End Function
    Private Function CreateEvent(ByVal resourceId As Object, ByVal subject As String, ByVal status As Integer, ByVal label As Integer) As CustomEvent
        Dim customEvent As New CustomEvent()
        customEvent.Subject = subject
        customEvent.OwnerId = resourceId
        Dim rnd As Random = DemoUtils.RandomInstance
        Dim rangeInHours As Integer = 48
        customEvent.StartTime = Date.Today + TimeSpan.FromHours(rnd.Next(0, rangeInHours))
        customEvent.EndTime = customEvent.StartTime.Add(TimeSpan.FromHours(rnd.Next(0, rangeInHours \ 8)))
        customEvent.Status = status
        customEvent.Label = label
        customEvent.Id = "ev" & customEvent.GetHashCode()
        Return customEvent
    End Function
    #End Region

    ' User generated appointment id
    Protected Sub ASPxScheduler1_AppointmentInserting(ByVal sender As Object, ByVal e As PersistentObjectCancelEventArgs)

        Dim storage_Renamed As ASPxSchedulerStorage = DirectCast(sender, ASPxSchedulerStorage)
        Dim apt As Appointment = CType(e.Object, Appointment)
        storage_Renamed.SetAppointmentId(apt, "a" & apt.GetHashCode())
    End Sub
    ' Populating ObjectDataSource
    Protected Sub appointmentsDataSource_ObjectCreated(ByVal sender As Object, ByVal e As ObjectDataSourceEventArgs)
        e.ObjectInstance = New CustomEventDataSource(GetCustomEvents())
    End Sub

    Protected Sub ASPxScheduler1_AllowAppointmentCreate(ByVal sender As Object, ByVal e As AppointmentOperationEventArgs)
        e.Allow = ASPxScheduler1.SelectedInterval.Start >= Date.Today
    End Sub

    Protected Sub ASPxScheduler1_VisibleIntervalChanged(ByVal sender As Object, ByVal e As EventArgs)
        ASPxScheduler1.ApplyChanges(ASPxSchedulerChangeAction.RenderViewMenu)
    End Sub

    Protected Sub ASPxScheduler1_AllowAppointmentDrag(ByVal sender As Object, ByVal e As AppointmentOperationEventArgs)
        e.Allow = e.Appointment.Start >= Date.Today
    End Sub

    Protected Sub ASPxScheduler1_BeforeExecuteCallbackCommand(ByVal sender As Object, ByVal e As SchedulerCallbackCommandEventArgs)
        If e.CommandId = "RecreateMenu" Then
            e.Command = New MyCallbackCommand(ASPxScheduler1)
        End If
    End Sub

    Public Class MyCallbackCommand
        Inherits SchedulerCallbackCommand

        Private scheduler As ASPxScheduler = Nothing

        Public Sub New(ByVal scheduler As ASPxScheduler)
            MyBase.New(scheduler)
            Me.scheduler = scheduler
        End Sub

        Public Overrides ReadOnly Property Id() As String
            Get
                Return "RecreateMenu"
            End Get
        End Property

        Protected Overrides Sub ExecuteCore()
            scheduler.ApplyChanges(ASPxSchedulerChangeAction.RenderAppointmentMenu)
        End Sub

        Protected Overrides Sub ParseParameters(ByVal parameters As String)

        End Sub
    End Class

    Public Class DemoUtils
        Public Shared RandomInstance As New Random()
        Public Shared Users() As String = { "Peter Dolan", "Ryan Fischer", "Andrew Miller", "Tom Hamlett", "Jerry Campbell", "Carl Lucas", "Mark Hamilton", "Steve Lee" }

        Public Shared Sub FillResources(ByVal storage As ASPxSchedulerStorage, ByVal count As Integer)
            Dim resources As ResourceCollection = storage.Resources.Items
            storage.BeginUpdate()
            Try
                Dim cnt As Integer = Math.Min(count, Users.Length)
                For i As Integer = 1 To cnt
                    resources.Add(storage.CreateResource(i, Users(i - 1)))
                Next i
            Finally
                storage.EndUpdate()
            End Try
        End Sub
    End Class
End Class
