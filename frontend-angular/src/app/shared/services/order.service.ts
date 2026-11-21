import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private http: HttpClient = inject(HttpClient);

  public getAll(): Observable<PagedResult<OrderListItem>> {
    return this.http.get<PagedResult<OrderListItem>>('/api/v1/orders');
  }
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface OrderListItem {
  id: string;
  orderNumber: string;
  customerName: string;
  createdAt: string;
  status: OrderStatus;
  itemsCount: number;
  totalAmount: number;
}

export enum OrderStatus {
  None = 0,
  New = 1,
  Confirmed = 2,
  Processing = 3,
  Completed = 4,
  Cancelled = 5,
}
