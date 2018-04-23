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

Partial Public Class AppointmentForm
	Inherits SchedulerFormControl
	Public ReadOnly Property CanShowReminders() As Boolean
		Get
			Return (CType(Parent, AppointmentFormTemplateContainer)).Control.Storage.EnableReminders
		End Get
	End Property

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		'PrepareChildControls();
		tbSubject.Focus()
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

		AppointmentRecurrenceForm1.Visible = container.ShouldShowRecurrence

		If container.Appointment.HasReminder Then
			cbReminder.Value = container.Appointment.Reminder.TimeBeforeStart.ToString()
			chkReminder.Checked = True
		Else
			cbReminder.ClientEnabled = False
		End If

		btnOk.ClientSideEvents.Click = container.SaveHandler
		btnCancel.ClientSideEvents.Click = container.CancelHandler
		btnDelete.ClientSideEvents.Click = container.DeleteHandler
		'btnDelete.Enabled = !container.IsNewAppointment;
	End Sub

	Protected Overrides Sub PrepareChildControls()
		Dim container As AppointmentFormTemplateContainer = CType(Parent, AppointmentFormTemplateContainer)
		Dim control As ASPxScheduler = container.Control

		AppointmentRecurrenceForm1.EditorsInfo = New EditorsInfo(control, control.Styles.FormEditors, control.Images.FormEditors, control.Styles.Buttons)
		MyBase.PrepareChildControls()
	End Sub
	Protected Overrides Function GetChildEditors() As ASPxEditBase()
		Dim edits() As ASPxEditBase = { lblSubject, tbSubject, lblLocation, tbLocation, lblLabel, edtLabel, lblStartDate, edtStartDate, lblEndDate, edtEndDate, lblStatus, edtStatus, lblAllDay, chkAllDay, lblResource, edtResource, tbDescription, cbReminder }
		Return edits
	End Function
	Protected Overrides Function GetChildButtons() As ASPxButton()
		Dim buttons() As ASPxButton = { btnOk, btnCancel, btnDelete }
		Return buttons
	End Function
End Class
