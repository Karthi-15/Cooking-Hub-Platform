import { Component, ElementRef, OnInit, ViewChild, AfterViewInit, Renderer2 } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, AfterViewInit {
  @ViewChild('statsSection', { static: true }) statsSection!: ElementRef;
  isAdmin: boolean = false;
  username: string = '';

  private backToTopBtn!: HTMLElement;

  constructor(private authService: AuthService, private el: ElementRef, private renderer: Renderer2) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    const userInfo = this.authService.getUserInfo();
    this.username = userInfo?.username || '';
  }

  ngAfterViewInit() {
    this.observeScrollAnimations();
    this.setupBackToTop();
  }

  observeStatsSection() {
    if (!this.statsSection) return;
    const observer = new IntersectionObserver(
      entries => {
        entries.forEach(entry => {
          if (entry.isIntersecting) {
            observer.disconnect();
          }
        });
      },
      { threshold: 0.5 }
    );
    observer.observe(this.statsSection.nativeElement);
  }
  observeScrollAnimations(): void {
    const animatedElements = this.el.nativeElement.querySelectorAll('.scroll-animate');
    if (!animatedElements || animatedElements.length === 0) return;
  
    let delayCounter = 0;
  
    const observer = new IntersectionObserver((entries, obs) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          const target = entry.target as HTMLElement;
          target.style.animationDelay = `${delayCounter * 0.1}s`;
          target.classList.add('animate-fadeInUp');
          obs.unobserve(target);
          delayCounter++;
        }
      });
    }, { threshold: 0.1 });
  
    animatedElements.forEach((el: Element) => observer.observe(el));
  }
  

  setupBackToTop() {
    this.backToTopBtn = this.el.nativeElement.querySelector('#back-to-top');
    if (!this.backToTopBtn) return;

    window.addEventListener('scroll', () => {
      if (window.scrollY > 300) {
        this.renderer.removeClass(this.backToTopBtn, 'opacity-0');
        this.renderer.removeClass(this.backToTopBtn, 'pointer-events-none');
        this.renderer.addClass(this.backToTopBtn, 'opacity-100');
      } else {
        this.renderer.addClass(this.backToTopBtn, 'opacity-0');
        this.renderer.addClass(this.backToTopBtn, 'pointer-events-none');
        this.renderer.removeClass(this.backToTopBtn, 'opacity-100');
      }
    });

    this.backToTopBtn.addEventListener('click', () => {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    });
  }
}
