# How to prevent a user from scheduling appointments in the past


<p>This example demonstrates how to prevent the end-user from creating appointments in the past. The main idea is to handle the ASPxScheduler's client-side SelectionChanged event and send a callback to the server to regenerate the control's context menu when a date in the past is selected.</p>

<br/>


