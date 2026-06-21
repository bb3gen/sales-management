import { createRouter, createWebHistory } from 'vue-router'

import LoginView from '@/views/LoginView.vue'
import HomeView from '@/views/HomeView.vue'
import CustomerListView from '@/views/CustomerListView.vue'
import CustomerCreateView from '@/views/CustomerCreateView.vue'
import CustomerEditView from '@/views/CustomerEditView.vue'
import MainLayout from '@/layouts/MainLayout.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/login',
      component: LoginView,
    },
    {
      path: '/',
      component: MainLayout,
      children: [
        {
          path: '',
          component: HomeView,
        },
        {
          path: 'customers',
          component: CustomerListView,
        },
        {
          path: 'customers/create',
          component: CustomerCreateView,
        },
        {
          path: 'customers/:id/edit',
          component: CustomerEditView,
        },
      ],
    },
  ],
})

export default router
