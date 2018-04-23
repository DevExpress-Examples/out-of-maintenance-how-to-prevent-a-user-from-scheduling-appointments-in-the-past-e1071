<%@ Page Language="vb" AutoEventWireup="true"  CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.ASPxScheduler.v15.2" Namespace="DevExpress.Web.ASPxScheduler" TagPrefix="dxwschs" %>
<%@ Register Assembly="DevExpress.XtraScheduler.v15.2.Core" Namespace="DevExpress.XtraScheduler" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<script type="text/javascript">
    var prevDate = null;
    function needRecreateMenu(currDate) {        
        if (prevDate == null) {
            prevDate = currDate;
            return false;
        }
        if ((isLessThanToday(currDate) && !isLessThanToday(prevDate)) ||
            (isLessThanToday(prevDate) && !isLessThanToday(currDate))) {
            prevDate = currDate;
            return true;
        }
    }

    function isLessThanToday(date) {
        var today = new Date();
        return (today.getYear() > date.getYear() ||
            (today.getYear() == date.getYear() && today.getMonth() > date.getMonth()) ||
            (today.getYear() == date.getYear() && today.getMonth() == date.getMonth() && today.getDate() > date.getDate()));        
    }

</script>
<body>
    <form id="form1" runat="server">
    <div>
        <dxwschs:ASPxScheduler ID="ASPxScheduler1" ClientInstanceName="ASPxScheduler1" runat="server" 
            Start="2008-10-28"
            OnAppointmentInserting="ASPxScheduler1_AppointmentInserting" 
            onallowappointmentcreate="ASPxScheduler1_AllowAppointmentCreate" 
            onallowappointmentdrag="ASPxScheduler1_AllowAppointmentDrag" 
            onvisibleintervalchanged="ASPxScheduler1_VisibleIntervalChanged" 
            onbeforeexecutecallbackcommand="ASPxScheduler1_BeforeExecuteCallbackCommand">                         
            <Views>
                <DayView><TimeRulers><cc1:TimeRuler /></TimeRulers></DayView>
                <WorkWeekView><TimeRulers><cc1:TimeRuler /></TimeRulers></WorkWeekView>
                <TimelineView />         
            </Views>
            <optionscustomization allowappointmentcreate="Custom" 
                allowappointmentdrag="Custom" />
            <clientsideevents selectionchanged="function(s, e) {
                if(needRecreateMenu(s.selection.interval.GetStart())){
                    //debugger;
                    ASPxScheduler1.RaiseCallback('RecreateMenu');
                }
}"          />
            <Storage EnableReminders="false"></Storage>
        </dxwschs:ASPxScheduler>
        <asp:ObjectDataSource ID="appointmentDataSource" runat="server" DataObjectTypeName="CustomEvent" TypeName="CustomEventDataSource" DeleteMethod="DeleteMethodHandler" SelectMethod="SelectMethodHandler" InsertMethod="InsertMethodHandler" UpdateMethod="UpdateMethodHandler" OnObjectCreated="appointmentsDataSource_ObjectCreated" />    
    </div>
    </form>
</body>
</html>