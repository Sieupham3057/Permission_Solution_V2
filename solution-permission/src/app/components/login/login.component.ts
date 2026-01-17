import { Component, inject, Input } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { UserLogin } from '../../models/user-login.model';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { NgClass } from '@angular/common';

export interface IError {
  title: string;
  status: string;
  detail: string;
  errors: string[];
}

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, NgClass],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  userLogin = new UserLogin();
  isLoading = false;
  formResetToggle = true;

  @Input()
  isModal = false;

  login() {
    this.isLoading = true;

    this.authService.login(this.userLogin).subscribe({
      next: user => {
        this.isLoading = false;
        this.reset();
        this.router.navigate(['/home']);
      },
      error: error => {
        alert('Username or password is not correct!');
        setTimeout(() => {
          this.isLoading = false;
        }, 200);
      }
    })
  }

  reset() {
    this.formResetToggle = false;

    setTimeout(() => {
      this.formResetToggle = true;
    });
  }

  showErrorAlert(caption: string, message: string) {
    alert(`${caption} ${message}`);
  }

}
