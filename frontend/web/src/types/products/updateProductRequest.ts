export interface UpdateProductRequest {
  productName: string
  unitPrice: number | null
  unit?: string
  remarks?: string
  updatedAt: string
}
