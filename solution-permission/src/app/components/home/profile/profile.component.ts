import { Component, inject, input, Input, OnInit, viewChild } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { Permissions } from '../../../models/permission.model';
import { User } from '../../../models/user.model';
import { Role } from '../../../models/role.model';
import { UserEdit } from '../../../models/user-edit.model';
import { HttpErrorResponse } from '@angular/common/http';
import { FormsModule, NgForm, NgModel } from '@angular/forms';
import { NgClass } from '@angular/common';
import { NgSelectModule } from '@ng-select/ng-select';
import { Utilities } from '../../../services/utilities';

@Component({
  selector: 'app-profile',
  imports: [FormsModule, NgClass, NgSelectModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {

  private accountService = inject(AccountService);
  public formResetToggle = true;
  public isEditMode = false;
  public isNewUser = false;
  public isSaving = false;
  public isChangePassword = false;
  public isEditingSelf = false;
  public showValidationErrors = false;
  public uniqueId = Utilities.generateGuid();
  public user = new User();
  public userEdit = new UserEdit();
  public allRoles: Role[] = [];

  readonly form = viewChild<NgForm>('f');
  readonly userName = viewChild<NgModel>('userName');
  readonly userPassword = viewChild<NgModel>('userPassword');
  readonly email = viewChild<NgModel>('email');
  readonly currentPassword = viewChild<NgModel>('currentPassword');
  readonly newPassword = viewChild<NgModel>('newPassword');
  readonly confirmPassword = viewChild<NgModel>('confirmPassword');
  readonly roles = viewChild<NgModel>('roles');

  @Input() isGeneralEditor = false;
  readonly isViewOnly = input(false);

  ngOnInit(): void {
    this.loadCurrentUser();
  }

  private loadCurrentUser() {
    if (this.canViewAllRoles) {
      this.accountService.getUserAndRoles()
        .subscribe({
          next: results => this.onCurrentUserDataLoadSuccessful(results[0], results[1]),
          error: error => this.onCurrentUserDataLoadFailed(error)
        });
    } else {
      this.accountService.getUser()
        .subscribe({
          next: user => this.onCurrentUserDataLoadSuccessful(user, user.roles.map(role => new Role(role))),
          error: error => this.onCurrentUserDataLoadFailed(error)
        });
    }
  }

  private onCurrentUserDataLoadSuccessful(user: User, roles: Role[]) {
    this.user = user;
    this.allRoles = roles;
  }

  private onCurrentUserDataLoadFailed(error: HttpErrorResponse) {
    alert(error);
    this.user = new User();
  }

  getRoleByName(name: string) {
    return this.allRoles.find((r) => r.name === name);
  }

  resetForm(replace = false) {
    this.isChangePassword = false;

    if (!replace) {
      this.form()?.reset();
    } else {
      this.formResetToggle = false;

      setTimeout(() => {
        this.formResetToggle = true;
      });
    }
  }

  save() {
    if (this.isNewUser) {
      this.accountService.newUser(this.userEdit)
        .subscribe({
          next: user => this.saveSuccessHelper(user),
          error: error => this.saveFailedHelper(error)
        });
    } else {
      this.accountService.updateUser(this.userEdit)
        .subscribe({
          next: () => this.saveSuccessHelper(),
          error: error => this.saveFailedHelper(error)
        });
    }
  }

  deletePasswordFromUser(user: UserEdit | User) {
    const userEdit = user as UserEdit;

    delete userEdit.currentPassword;
    delete userEdit.newPassword;
    delete userEdit.confirmPassword;
  }

  private saveSuccessHelper(user?: User) {
    this.testIsRoleUserCountChanged(this.user, this.userEdit);

    if (user) {
      Object.assign(this.userEdit, user);
    }

    this.isSaving = false;
    this.isChangePassword = false;
    this.showValidationErrors = false;

    this.deletePasswordFromUser(this.userEdit);
    Object.assign(this.user, this.userEdit);
    this.userEdit = new UserEdit();
    this.resetForm();
    this.isEditMode = false;
  }

  private saveFailedHelper(error: HttpErrorResponse) {
    this.isSaving = false;
  }

  private testIsRoleUserCountChanged(currentUser: User, editedUser: User) {
    const rolesAdded = this.isNewUser ? editedUser.roles : editedUser.roles.filter(role => currentUser.roles.indexOf(role) === -1);
    const rolesRemoved = this.isNewUser ? [] : currentUser.roles.filter(role => editedUser.roles.indexOf(role) === -1);

    const modifiedRoles = rolesAdded.concat(rolesRemoved);

    if (modifiedRoles.length) {
      setTimeout(() => this.accountService.onRolesUserCountChanged(modifiedRoles));
    }
  }

  changePassword() {
    this.isChangePassword = true;
  }

  edit() {
    if (!this.isGeneralEditor) {
      this.isEditingSelf = true;
      this.userEdit = new UserEdit();
      Object.assign(this.userEdit, this.user);
    } else {
      if (!this.userEdit) {
        this.userEdit = new UserEdit();
      }

      this.isEditingSelf = this.accountService.currentUser ? this.userEdit.id === this.accountService.currentUser.id : false;
    }

    this.isEditMode = true;
    this.showValidationErrors = true;
    this.isChangePassword = false;
  }

  showErrorAlert(caption: string, message: string) {
    alert(`${caption} - ${message}`)
  }

  showValidationAlerts() {
    if (!this.userName()?.valid)
      this.showErrorAlert('User name is required', 'Please enter a user name (minimum of 2 and maximum of 200 characters)');

    if (this.userPassword() && !this.userPassword()?.valid)
      this.showErrorAlert('Password is required', 'Please enter the current password');

    if (this.email()?.errors?.['required'])
      this.showErrorAlert('Email is required', 'Please enter an email address (maximum of 200 characters)');

    if (this.email()?.errors?.['pattern'])
      this.showErrorAlert('Invalid Email', 'Please enter a valid email address');

    if (this.isChangePassword && this.isEditingSelf && !this.currentPassword()?.valid)
      this.showErrorAlert('Current password is required', 'Please enter the current password');

    if ((this.isChangePassword || this.isNewUser) && !this.newPassword()?.valid)
      this.showErrorAlert('New password is required', 'Please enter the new password (minimum of 6 characters)');

    if ((this.isChangePassword || this.isNewUser) && this.newPassword()?.valid && this.confirmPassword()?.errors?.['required'])
      this.showErrorAlert('Confirmation password is required', 'Please enter the confirmation password');

    if ((this.isChangePassword || this.isNewUser) && this.newPassword()?.valid && this.confirmPassword()?.errors?.['validateEqual'])
      this.showErrorAlert('Passwword mismatch', 'New password and confirmation password do not match');

    if (this.canAssignRoles && !this.roles()?.valid)
      this.showErrorAlert('Roles is required', 'Please select a minimum of 1 role');
  }

  unlockUser() {
    this.isSaving = true;
    this.accountService.unblockUser(this.userEdit.id)
      .subscribe({
        next: () => {
          this.isSaving = false;
          this.userEdit.isLockedOut = false;
        },
        error: error => {
          this.isSaving = false;
        }
      });
  }

  close() {
    this.userEdit = this.user = new UserEdit();
    this.showValidationErrors = false;
    this.resetForm();
    this.isEditMode = false;
  }

  cancel() {
    if (this.isGeneralEditor) {
      this.userEdit = this.user = new UserEdit();
    } else {
      this.userEdit = new UserEdit();
    }

    this.showValidationErrors = false;
    this.resetForm();

    if (!this.isGeneralEditor) {
      this.isEditMode = false;
    }
  }

  get canViewAllRoles() {
    return this.accountService.userHasPermission(Permissions.viewRoles);
  }

  get canAssignRoles() {
    return this.accountService.userHasPermission(Permissions.assignRoles);
  }
}
