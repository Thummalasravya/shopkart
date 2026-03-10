import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-star-rating',
  standalone: true,          // ✅ REQUIRED
  imports: [CommonModule],   // ✅ REQUIRED
  template: `
    <span class="star" *ngFor="let star of stars">
      ★
    </span>
  `,
  styles: [`
    .star {
      color: gold;
      font-size: 18px;
    }
  `]
})
export class StarRatingComponent {
  @Input() rating = 0;

  get stars() {
    return Array(this.rating);
  }
}
