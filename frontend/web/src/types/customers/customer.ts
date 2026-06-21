export interface Customer {
  id: string
  customerCode: string
  customerName: string
  customerKana?: string
  postalCode?: string
  address?: string
  phoneNumber?: string
  email?: string

  createdAt: string
  updatedAt: string
}
