import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

import LoginView from '@/views/LoginView.vue'
import MainLayout from '@/layouts/MainLayout.vue'

import HomeView from '@/views/HomeView.vue'

import CustomerListView from '@/views/customers/CustomerListView.vue'
import CustomerCreateView from '@/views/customers/CustomerCreateView.vue'
import CustomerEditView from '@/views/customers/CustomerEditView.vue'

import ProductListView from '@/views/products/ProductListView.vue'
import ProductCreateView from '@/views/products/ProductCreateView.vue'
import ProductEditView from '@/views/products/ProductEditView.vue'

import OrderListView from '@/views/orders/OrderListView.vue'
import OrderCreateView from '@/views/orders/OrderCreateView.vue'
import OrderEditView from '@/views/orders/OrderEditView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: {
        guestOnly: true,
      },
    },
    {
      path: '/',
      component: MainLayout,
      meta: {
        requiresAuth: true,
      },
      children: [
        // ホーム
        {
          path: '',
          name: 'home',
          component: HomeView,
        },
        // 顧客
        {
          path: 'customers',
          name: 'customers',
          component: CustomerListView,
        },
        {
          path: 'customers/create',
          name: 'customer-create',
          component: CustomerCreateView,
        },
        {
          path: 'customers/:id/edit',
          name: 'customer-edit',
          component: CustomerEditView,
        },
        // 商品
        {
          path: 'products',
          name: 'products',
          component: ProductListView,
        },
        {
          path: 'products/create',
          name: 'product-create',
          component: ProductCreateView,
        },
        {
          path: 'products/:id/edit',
          name: 'product-edit',
          component: ProductEditView,
        },
        // 注文
        {
          path: 'orders',
          name: 'orders',
          component: OrderListView,
        },
        {
          path: 'orders/create',
          name: 'order-create',
          component: OrderCreateView,
        },
        {
          path: 'orders/:id/edit',
          name: 'order-edit',
          component: OrderEditView,
        },
      ],
    },
  ],
})

router.beforeEach((to) => {
  const authStore = useAuthStore()

  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth)

  // 認証が必要、アクセストークンなし⇒ログイン画面へ
  if (requiresAuth && !authStore.accessToken) {
    return '/login'
  }

  // ログイン画面、アクセストークンあり⇒ホーム画面へ
  if (to.meta.guestOnly && authStore.accessToken) {
    return '/'
  }

  return true
})

export default router
