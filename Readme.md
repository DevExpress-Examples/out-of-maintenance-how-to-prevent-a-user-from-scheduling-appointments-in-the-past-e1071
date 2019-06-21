<!-- default file list -->
*Files to look at*:

* [CustomEvents.cs](./CS/WebSite/App_Code/CustomEvents.cs) (VB: [CustomEvents.vb](./VB/WebSite/App_Code/CustomEvents.vb))
* [Default.aspx](./CS/WebSite/Default.aspx) (VB: [Default.aspx](./VB/WebSite/Default.aspx))
* [Default.aspx.cs](./CS/WebSite/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/WebSite/Default.aspx.vb))
<!-- default file list end -->
# How to prevent a user from scheduling appointments in the past
<!-- run online -->
**[[Run Online]](https://codecentral.devexpress.com/e1071/)**
<!-- run online end -->


<p>This example demonstrates how to prevent the end-user from creating appointments in the past. The main idea is to handle the ASPxScheduler's client-side SelectionChanged event and send a callback to the server to regenerate the control's context menu when a date in the past is selected.</p>

<br/>


