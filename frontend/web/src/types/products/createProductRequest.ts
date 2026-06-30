export interface CreateProductRequest {
  productName: string
  unitPrice: number | null
  unit?: string
  remarks?: string
}
