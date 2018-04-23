'
'{************************************************************************************}
'{                                                                                    }
'{   DO NOT MODIFY THIS FILE!                                                         }
'{                                                                                    }
'{   It will be overwritten without prompting when a new version becomes              }
'{   available. All your changes will be lost.                                        }
'{                                                                                    }
'{   This file contains the default template and is required for the form             }
'{   rendering. Improper modifications may result in incorrect behavior of            }
'{   the appointment form.                                                            }
'{                                                                                    }
'{   In order to create and use your own custom template, perform the following       }
'{   steps:                                                                           }
'{       1. Save a copy of this file with a different name in another location.       }
'{       2. Specify the file location as the 'OptionsForms.AppointmentFormTemplateUrl'}
'{          property of the ASPxScheduler control.                                    }
'{       3. If you need custom fields to be displayed and processed, you should       }
'{          accomplish steps 4-9; otherwise, go to step 10.                           }
'{       4. Create a class, derived from the AppointmentFormTemplateContainer,        }
'{          containing custom properties. This class definition can be located        }
'{          within a class file in the App_Code folder.                               }
'{       5. Replace AppointmentFormTemplateContainer references in the template       }
'{          page with the name of the class you've created in step 4.                 }
'{       6. Handle the AppointmentFormShowing event to create an instance of the      }
'{          template container class, defined in step 4, and specify it as the        }
'{          destination container instead of the default one.                         }
'{       7. Define a class, which inherits from the                                   }
'{          DevExpress.Web.ASPxScheduler.Internal.AppointmentFormController.          }
'{          This class provides data exchange between the form and the appointment.   }
'{          You should override ApplyCustomFieldsValues() method of the base class.   }
'{       8. Define a class, which inherits from the                                   }
'{          DevExpress.Web.ASPxScheduler.Internal.AppointmentFormSaveCallbackCommand. }
'{          This class creates an instance of the AppointmentFormController inheritor }
'{          (defined in step 7) via the CreateAppointmentFormController method and    }
'{          overrides the AssignControllerValues method  of the base class to collect }
'{          user data from the form's editors.                                        }
'{       9. Handle the BeforeExecuteCallbackCommand event. The event handler code     }
'{          should create an instance of the class defined in step 8, and specify it  }
'{          as the destination command instead of the default one.                    }
'{      10. Modify the overall appearance of the page and its layout.                 }
'{                                                                                    }
'{************************************************************************************}
'

Imports Microsoft.VisualBasic
Imports System
Imports System.Web.UI
Imports DevExpress.XtraScheduler
Imports DevExpress.Web.ASPxEditors
Imports DevExpress.Web.ASPxScheduler
Imports DevExpress.Web.ASPxScheduler.Internal
Imports DevExpress.Web.ASPxScheduler.Controls
Imports DevExpress.Web.ASPxClasses

Partial Public Class AppointmentFormEx
	Inherits SchedulerFormControl
	Private recurrenceControl_Renamed As AppointmentRecurrenceControl

	Public ReadOnly Property CanShowReminders() As Boolean
		Get
			Return (CType(Parent, AppointmentFormTemplateContainer)).Control.Storage.EnableReminders
		End Get
	End Property
	Public ReadOnly Property RecurrenceControl() As AppointmentRecurrenceControl
		Get
			Return recurrenceControl_Renamed
		End Get
	End Property

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		RenderRecurrenceControl(False)
		'PrepareChildControls();
		tbSubject.Focus()
	End Sub
	Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
		Dim container As AppointmentFormTemplateContainer = CType(Parent, AppointmentFormTemplateContainer)
		Dim control As ASPxScheduler = container.Control
		RecurrenceControl.EditorsInfo = New EditorsInfo(control, control.Styles.FormEditors, control.Images.FormEditors, control.Styles.Buttons)
		RecurrenceControl.Visible = chkRecurrence.Checked AndAlso chkRecurrence.Visible
		MyBase.Render(writer)
	End Sub
	Public Overrides Sub DataBind()
		MyBase.DataBind()
		Dim container As AppointmentFormTemplateContainer = CType(Parent, AppointmentFormTemplateContainer)
		Dim apt As Appointment = container.Appointment
		edtLabel.SelectedIndex = apt.LabelId
		edtStatus.SelectedIndex = apt.StatusId
		If (Not Object.Equals(apt.ResourceId, Resource.Empty.Id)) Then
			edtResource.Value = apt.ResourceId.ToString()
		Else
			edtResource.Value = SchedulerIdHelper.EmptyResourceId
		End If

		chkRecurrence.Visible = container.ShouldShowRecurrence
		'AppointmentRecurrenceForm1.Visible = container.ShouldShowRecurrence;

		If container.Appointment.HasReminder Then
			cbReminder.Value = container.Appointment.Reminder.TimeBeforeStart.ToString()
			chkReminder.Checked = True
		Else
			cbReminder.ClientEnabled = False
		End If

		btnOk.ClientSideEvents.Click = container.SaveHandler
		btnCancel.ClientSideEvents.Click = container.CancelHandler
		btnDelete.ClientSideEvents.Click = container.DeleteHandler
	End Sub
	Protected Overrides Function GetChildEditors() As ASPxEditBase()
		Dim edits() As ASPxEditBase = { lblSubject, tbSubject, lblLocation, tbLocation, lblLabel, edtLabel, lblStartDate, edtStartDate, lblEndDate, edtEndDate, lblStatus, edtStatus, lblAllDay, chkAllDay, lblResource, edtResource, tbDescription, cbReminder }
		Return edits
	End Function
	Protected Overrides Function GetChildButtons() As ASPxButton()
		Dim buttons() As ASPxButton = { btnOk, btnCancel, btnDelete }
		Return buttons
	End Function
	Protected Sub OnCallback(ByVal sender As Object, ByVal e As CallbackEventArgsBase)
		RenderRecurrenceControl(True)
	End Sub
	Protected Sub RenderRecurrenceControl(ByVal isRecurring As Boolean)
		If Me.recurrenceControl_Renamed IsNot Nothing Then
			Me.recurrenceControl_Renamed.Visible = isRecurring
			Return
		End If
		Dim container As AppointmentFormTemplateContainer = CType(Parent, AppointmentFormTemplateContainer)
		Me.recurrenceControl_Renamed = New AppointmentRecurrenceControl()
		recurrenceControl_Renamed.ID = "RecurrenceControl1"
		recurrenceControl_Renamed.Visible = isRecurring 'container.ShouldShowRecurrence;
		recurrenceControl_Renamed.DayNumber = container.RecurrenceDayNumber
		recurrenceControl_Renamed.End = container.RecurrenceEnd
		recurrenceControl_Renamed.Month = container.RecurrenceMonth
		recurrenceControl_Renamed.OccurrenceCount = container.RecurrenceOccurrenceCount
		recurrenceControl_Renamed.Periodicity = container.RecurrencePeriodicity
		recurrenceControl_Renamed.RecurrenceRange = container.RecurrenceRange
		recurrenceControl_Renamed.Start = container.RecurrenceStart
		recurrenceControl_Renamed.WeekDays = container.RecurrenceWeekDays
		recurrenceControl_Renamed.WeekOfMonth = container.RecurrenceWeekOfMonth
		recurrenceControl_Renamed.RecurrenceType = container.RecurrenceType
		recurrenceControl_Renamed.IsFormRecreated = container.IsFormRecreated
		RecurrencePanel.Controls.Add(recurrenceControl_Renamed)
		recurrenceControl_Renamed.EditorsInfo = New EditorsInfo(container.Control, container.Control.Styles.FormEditors, container.Control.Images.FormEditors, container.Control.Styles.Buttons)
	End Sub
End Class
