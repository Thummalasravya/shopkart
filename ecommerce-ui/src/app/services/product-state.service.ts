import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductStateService {

  private searchSubject = new BehaviorSubject<string>('');
  private categorySubject = new BehaviorSubject<string>('All');

  search$ = this.searchSubject.asObservable();
  category$ = this.categorySubject.asObservable();

  setSearch(text: string) {
    this.searchSubject.next(text.toLowerCase());
  }

  setCategory(category: string) {
    this.categorySubject.next(category);
  }
}
