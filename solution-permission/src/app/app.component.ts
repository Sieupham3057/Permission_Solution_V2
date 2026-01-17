import { AfterViewChecked, Component, ElementRef, inject, OnInit, Renderer2 } from '@angular/core';
import { NavigationEnd, NavigationStart, Router, RouterOutlet } from '@angular/router';
import { BodyClassService } from './services/body-class.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit, AfterViewChecked {

  title = 'solution-permission';
  private router = inject(Router);
  private bodyClassService = inject(BodyClassService);
  private renderer = inject(Renderer2);
  private elementRef = inject(ElementRef);

  ngAfterViewChecked(): void {
    // Check if the script is already added to prevent duplicates
    //if (document.getElementById('customJsScript')) return;

    const script = this.renderer.createElement('script');
    this.renderer.setAttribute(script, 'type', 'text/javascript');
    this.renderer.setAttribute(script, 'id', 'customJsScript');
    this.renderer.setAttribute(script, 'src', 'assets/js/custom.min.js'); // Adjust path accordingly

    this.renderer.appendChild(this.elementRef.nativeElement, script);
  }

  ngOnInit(): void {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        // Set default body class for all routes
        this.bodyClassService.removeClass('login');
        this.bodyClassService.addClass('nav-md');  // Default class for layout page
      }

      if (event instanceof NavigationEnd) {
        // Check if we're on the login page and update the body class accordingly
        if (this.router.url === '/login') {
          this.bodyClassService.removeClass('nav-md');
          this.bodyClassService.addClass('login'); // Add 'login' class for login page
        } else {
          this.bodyClassService.removeClass('login');
          this.bodyClassService.addClass('nav-md'); // Ensure 'nav-md' class is re-added
        }
      }
    });
  }



}
