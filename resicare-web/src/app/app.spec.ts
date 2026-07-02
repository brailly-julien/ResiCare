import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    localStorage.clear(); // pas de session restaurée -> barre d'outils minimale
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        provideRouter([]), // le shell contient un <router-outlet>
        provideHttpClient(), // App -> AuthService -> HttpClient
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render the ResiCare brand', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('ResiCare');
  });
});
