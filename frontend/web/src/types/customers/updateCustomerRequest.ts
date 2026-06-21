export interface UpdateCustomerRequest {
  customerName: string
  customerKana?: string
  postalCode?: string
  address?: string
  phoneNumber?: string
  email?: string

  updatedAt: string
}
