import Vue from 'vue'
import App from './App.vue'

import Router from 'vue-router'

import Randomizer from './pages/Randomizer'
import Report from './pages/Report'
import Axios from 'axios'

import Vuex from './js/vuex'

Vue.config.productionTip = false
Vue.use(Router)

const router = new Router({
  routes: [
    { path: '/', component: Randomizer, meta: { title: 'Randomizer' } },
    { path: '/Report', component: Report, meta: { title: 'Randomizer Report' } },
  ],
  mode: 'history'
})

router.beforeEach((to, from, next) => {
  document.title = to.meta.title

  Vuex.state.pageTitle = to.meta.title

  if (to.meta.title == 'Randomizer') {
    Vuex.state.canLoadReport = false
  }

  next()
})

const axios = Axios
axios.defaults.baseURL = 'https://localhost:5001'

Vue.prototype.$axios = axios
Vue.prototype.$sleep = function(ms) {
  return new Promise(r => setTimeout(r, ms));
}

new Vue({
  render: h => h(App),
  router,
  store: Vuex
}).$mount('#app')
