import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { User } from '../../../models/user.model';
import { Permissions } from '../../../models/permission.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Role } from '../../../models/role.model';
import { HttpErrorResponse } from '@angular/common/http';
import { NgSelectModule } from '@ng-select/ng-select';
import { UserEdit } from '../../../models/user-edit.model';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [CommonModule, FormsModule, NgSelectModule],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss',
})
export class UserComponent implements OnInit {
  private accountService = inject(AccountService);

  users: User[] = [];
  currentUser: User = new User(); // User in login
  userEdit = new UserEdit();
  public isChangePassword = false;
  allRoles: Role[] = [];
  currentUseRoles: Role[] = [];

  showForm = false;
  isEditMode = false;
  editIndex: number | null = null;

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    if (this.canViewRoles) {
      this.accountService.getUsersAndRoles() // List users and all roles
        .subscribe({
          next: results => this.onDataLoadSuccessful(results[0], results[1]),
          error: error => this.onDataLoadFailed(error)
        });

      this.accountService.getUserAndRoles() // Load currentUser in case canViewRoles = true
        .subscribe({
          next: results => this.onCurrentUserDataLoadSuccessful(results[0], results[1]),
          error: error => this.onCurrentUserDataLoadFailed(error)
        });

    } else {
      this.accountService.getUsers() // List users and current user's roles
        .subscribe({
          next: users => this.onDataLoadSuccessful(users, this.accountService.currentUser?.roles.map(x => new Role(x)) ?? []),
          error: error => this.onDataLoadFailed(error)
        });

      this.accountService.getUser() // Load currentUser in case canViewRoles = false
        .subscribe({
          next: user => this.onCurrentUserDataLoadSuccessful(user, user.roles.map(role => new Role(role))),
          error: error => this.onCurrentUserDataLoadFailed(error)
        });
    }
  }

  private onCurrentUserDataLoadSuccessful(user: User, roles: Role[]) {
    this.currentUser = user;
    this.currentUseRoles = roles;
  }

  private onCurrentUserDataLoadFailed(error: HttpErrorResponse) {
    this.currentUser = new User();
  }

  onDataLoadSuccessful(users: User[], roles: Role[]) {
    this.users = users;
    this.currentUseRoles = roles;
  }

  onDataLoadFailed(error: HttpErrorResponse) {
    alert(error);
  }

  onNewUser() {
    this.userEdit = new UserEdit();
    this.showForm = true;
    this.isEditMode = false;
    this.editIndex = null;
    this.isChangePassword = true;
  }

  onEditUser(user: User, index: number) {
    this.userEdit = new UserEdit();
    Object.assign(this.userEdit, user);
    this.showForm = true;
    this.isEditMode = true;
    this.editIndex = index;
    this.isChangePassword = false;
  }

  changePassword() {
    this.isChangePassword = true;
  }

  deletePasswordFromUser(user: UserEdit | User) {
    const userEdit = user as UserEdit;

    delete userEdit.currentPassword;
    delete userEdit.newPassword;
    delete userEdit.confirmPassword;
  }

  onSaveUser() {
    // this.accountService.saveUser(this.user).subscribe(saved => {
    //   if (this.isEditMode && this.editIndex !== null) {
    //     this.users[this.editIndex] = saved;
    //   } else {
    //     this.users.push(saved);
    //   }
    //   this.cancelForm();
    // });

    if (this.isEditMode && this.editIndex !== null) {
      alert('Add: ' + JSON.stringify(this.userEdit));
    }
    else {
      alert('Edit: ' + JSON.stringify(this.userEdit));
    }
    this.cancelForm();
  }

  onDeleteUser(user: User) {
    const result = confirm(`Are you sure you want to delete user: ${user.fullName}?`);
    if (result) {
      this.accountService.deleteUser(user.id).subscribe(() => {
        this.users = this.users.filter(u => u.id !== user.id);
      });
    }
  }

  cancelForm() {
    this.showForm = false;
    this.userEdit = new UserEdit();
    this.editIndex = null;
    this.isChangePassword = false;
  }

  get canAssignRoles() {
    return this.accountService.userHasPermission(Permissions.assignRoles);
  }

  get canViewRoles() {
    return this.accountService.userHasPermission(Permissions.viewRoles);
  }

  get canManageUsers() {
    return this.accountService.userHasPermission(Permissions.manageUsers);
  }

  get isNewUser() {
    return this.currentUser.userName === this.userEdit.userName;
  }

}
