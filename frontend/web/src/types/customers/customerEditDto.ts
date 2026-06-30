export interface CustomerEditDto {
  id: string
  customerCode: string
  customerName: string
  customerKana?: string
  postalCode?: string
  address?: string
  phoneNumber?: string
  email?: string
  updatedAt: string
}
