export interface OrderSearchRequest {
  page: number
  pageSize: number
  orderNumber?: string
  customerId?: string
  orderDateFrom?: string
  orderDateTo?: string
  status?: number
}
