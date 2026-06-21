// 検索依頼
export interface CustomerSearchRequest {
  customerCode?: string
  customerName?: string
  page: number
  pageSize: number
}
