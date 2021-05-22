<template>
  <div class="report-main">
    <table v-if="reportData">
      <tr>
        <th>Numeric</th>
        <th>Alphanumeric</th>
        <th>Float</th>
      </tr>
      <tr>
        <td>{{ reportData['numericPerc'].toFixed(2) }}%</td>
        <td>{{ reportData['alphanumericPerc'].toFixed(2) }}%</td>
        <td>{{ reportData['floatPerc'].toFixed(2) }}%</td>
      </tr>
    </table>
    <div class="data-title">The first 20 data are listed below</div>
    <table v-if="reportData">
      <tr v-for="(r, ri) in reportList" :key="ri">
        <td v-for="(o, oi) in Object.keys(r)" :class="[ oi == 0 ? 'left-td' : 'right-td' ]" :key="oi">{{ r[o] }} {{ oi == 0 ? ':' : '' }}</td>
      </tr>
    </table>
  </div>
</template>

<script>
export default {
  components: {
  },
  data: function() {
    return {
      reportData: null,
      reportList: []
    }
  },
  props: {
  },
  methods: {
  },
  async mounted() {
    if (!this.$store.state.canLoadReport) {
      this.$router.push('/');
      return;
    }
    let report = await this.$axios.get('/apiv1/GetReport');
    this.reportData = report.data;
    if (this.reportData['fullReport'].length <= 20) {
      this.reportList = this.reportData['fullReport'];
    } else {
      this.reportList = this.reportData['fullReport'].slice(0, 20);
    }
  }
};
</script>

<style lang="scss" scoped>
.report-main {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
  padding: 10px;
  max-width: 500px;

  > table {
    width: 100%;
    border-collapse: collapse;
    table-layout: fixed;
    font-size: 0.9em;

    > tr > td, th {
      border: 1px solid black;
      padding: 5px;
      text-align: center;
      word-wrap: break-word;
    }

    > tr > td.left-td {
      text-align: right;
    }

    > tr > td.right-td {
      text-align: left;
    }

    > tr > td.left-td, td.right-td {
      border: none;
    }
  }

  > .data-title {
    margin: 15px 0;
  }
}
</style>