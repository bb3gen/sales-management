import { createRouter, createWebHistory } from 'vue-router'

import LoginView from '@/views/LoginView.vue'
import HomeView from '@/views/HomeView.vue'
import CustomerListView from '@/views/CustomerListView.vue'
import CustomerCreateView from '@/views/CustomerCreateView.vue'
import CustomerEditView from '@/views/CustomerEditView.vue'
import MainLayout from '@/layouts/MainLayout.vue'
import { useAuthStore } from '@/stores/authStore'

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
        {
          path: '',
          name: 'home',
          component: HomeView,
        },
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
