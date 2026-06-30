export interface ProductEditDto {
  id: string
  productCode: string
  productName: string
  unitPrice: number | null
  unit?: string
  remarks?: string
  updatedAt: string
}
