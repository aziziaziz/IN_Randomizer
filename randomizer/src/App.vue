<template>
  <div class="app-main">
    <div class="title">{{ $store.state.pageTitle }}</div>
    <router-view />
  </div>
</template>

<script>
export default {
  components: {
  },
  data: function() {
    return {
    }
  },
  props: {
  },
  methods: {
  },
  mounted() {
  }
};
</script>

<style lang="scss" scoped>
.app-main {
  width: 100%;
  display: flex;
  align-items: center;
  flex-direction: column;

  > * {
    box-sizing: border-box;
  }

  > .title {
    font-size: 2em;
    font-weight: bolder;
    margin: 10px 0;
  }
}
</style>

<style lang="scss">
body {
  margin: 0;
  font-family: 'Work Sans', sans-serif;
}
</style>