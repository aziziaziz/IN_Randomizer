import Vue from 'vue';
import VueX from 'vuex';

Vue.use(VueX);

export default new VueX.Store({
  state: {
    pageTitle: '',
    canLoadReport: false
  }
});