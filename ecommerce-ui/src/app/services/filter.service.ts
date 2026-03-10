import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FilterService {

  // current selected values
  searchText: string = '';
  selectedCategory: string = 'All';

  // observables
  private searchSource = new BehaviorSubject<string>('');
  private categorySource = new BehaviorSubject<string>('All');

  search$ = this.searchSource.asObservable();
  category$ = this.categorySource.asObservable();

  setSearch(text: string) {
    this.searchText = text.toLowerCase();
    this.searchSource.next(this.searchText);
  }

  setCategory(category: string) {
    this.selectedCategory = category;
    this.categorySource.next(this.selectedCategory);
  }
}
