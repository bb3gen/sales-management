import type { OrderDetailDto } from './OrderDetailDto'

export function createEmptyOrderDetail(lineNo: number): OrderDetailDto {
  return {
    id: '',
    lineNo,

    productId: '',
    productCode: '',
    productName: '',

    quantity: 1,

    unit: '',

    unitPrice: 0,

    amount: 0,
  }
}
