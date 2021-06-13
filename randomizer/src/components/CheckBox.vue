<template>
  <div class="checkbox-main">
    <input type="checkbox" :id="value" v-model="checked">
    <label :for="value">{{ value }}</label>
  </div>
</template>

<script>
export default {
  components: {
  },
  data: function() {
    return {
      checked: false
    }
  },
  props: {
    isChecked: { type: Boolean, default: false },
    value: { type: String, default: '' },
  },
  methods: {
  },
  mounted() {
    if (this.isChecked) {
      this.checked = this.isChecked;
    }
  },
  watch: {
    checked: function(val) {
      this.$emit('update:isChecked', val);
    }
  }
};
</script>

<style lang="scss" scoped>
.checkbox-main {
  display: flex;
  align-items: center;
  padding: 3px;
}
</style>