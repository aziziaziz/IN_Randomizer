<template>
  <div class="randomizer-main">
    <div class="title">This page will let you generate a randomize report</div>
    <div class="setup-section">
      <div class="setup-left">
        <div class="title">Set the data distribution</div>
        <div v-for="(title, index) in distributionTitle" :key="index" class="checkbox-section">
          <CheckBox :isChecked.sync="distributionCheck[index]" :value="title" />
          <input type="number" v-model.number="distributionValue[index]" @input="distributionChanged(index)" :disabled="randomObjects.length > 0 || !distributionCheck[index]" max="100" min="0"> %
        </div>
      </div>
      <div class="splitter"></div>
      <div class="setup-right">
        <div class="title">Size of the output file (KB)</div>
        <input type="Number" v-model.number="outputSize" :disabled="randomObjects.length > 0">
      </div>
    </div>
    <div class="delay-section">
      <div>Delay between each generated object (1000 = 1 second)</div>
      <input type="number" v-model.number="delay" :disabled="randomObjects.length > 0">
    </div>
    <div v-if="error" class="error">{{ error }}</div>
    <div class="button-section">
      <button @click="startClicked" :disabled="isRunning">{{ randomObjects.length > 0 ? 'Reset' : 'Start' }}</button>
      <button @click="stopClicked" :disabled="!isRunning">Stop</button>
    </div>

    <div v-if="isRunning || randomObjects.length > 0" class="random-section">
      <table>
        <tr v-for="(dt, dtIndex) in distributionTitle" :key="dtIndex">
          <td>{{ dt }}</td>
          <td>:</td>
          <td>{{ randomCounter[dtIndex] }}</td>
        </tr>
        <tr class="border-top">
          <td>Total</td>
          <td>:</td>
          <td>{{ randomCounter.reduce((a, b) => a + b) }}</td>
        </tr>
      </table>

      <div class="current-size">
        Generated size : 
        <span style="width: 100px; text-align: right">{{ (randomObjects.join(',').length / 1000) }} KB</span>
      </div>

      <button style="width: 200px; margin-top: 30px" :disabled="isRunning" @click="generateReport">Generate Report</button>
    </div>
  </div>
</template>

<script>
export default {
  components: {
  },
  data: function() {
    return {
      distributionTitle: [ 'Alphanumeric', 'Numeric', 'Float' ],
      distributionCheck: [ true, true, true ],
      distributionValue: [ 33, 33, 33 ],

      checkData: [],
      outputSize: '',
      
      randomObjects: [],
      randomCounter: [ 0, 0, 0 ],

      delay: 0,

      isRunning: false,

      error: ''
    }
  },
  props: {
  },
  methods: {
    startClicked: async function() {
      this.error = '';

      if (this.distributionCheck.filter(d => d).length == 0) {
        this.error = 'Please check at least one for the data distribution';
        return;
      }

      if (this.randomObjects.length == 0) {
        this.isRunning = true;
        while (this.isRunning) {
          await this.getRandom();
          await this.$sleep(this.delay);
        }
      } else {
        this.randomObjects = [];
        this.randomCounter = [ 0, 0, 0 ];
      }
    },
    stopClicked: function() {
      this.isRunning = false;
    },
    getRandom: async function() {
      let random = await this.$axios.get(`/apiv1/GetRandom?distribution=${this.getDistribution()}`);
      this.randomCounter[this.distributionTitle.indexOf(random.data['type'])]++;
      
      this.randomObjects.push(random.data['value']);
      if (this.outputSize > 0 && this.randomObjects.join(',').length / 1000 >= this.outputSize) {
        this.stopClicked();
      }
    },
    getDistribution: function() {
      let checked = this.getCheckedIndex(true);
      let dist = [];
      
      if (this.randomObjects.length > 0) {
        checked.checked.forEach((c) => {
          if (this.randomCounter[c] / this.randomObjects.length * 100 <= this.distributionValue[c]) {
            dist.push(c);
          }
        });
      } else {
        dist = checked.checked;
      }

      return dist.join(',');
    },
    generateReport: async function() {
      let generate = await this.$axios.post('/apiv1/GenerateReport', this.randomObjects.join(','));
      if (generate.data) {
        this.$store.state.canLoadReport = true;
        this.$router.push('/Report');
      }
    },
    getCheckedIndex: function(checkEmpty) {
      let checked = [];
      let unchecked = [];

      if (checkEmpty) {
        this.distributionCheck.forEach((d, i) => {
          if (d && this.distributionValue[i]) {
            checked.push(i);
          } else {
            unchecked.push(i);
          }
        });
      } else {
        this.distributionCheck.forEach((d, i) => d ? checked.push(i) : unchecked.push(i));
      }

      return { checked: checked, unchecked: unchecked };
    },
    checkPercentage: function() {
      let checked = this.getCheckedIndex();
      let perc = 100 / checked.checked.length;

      checked.checked.forEach(c => this.distributionValue[c] = perc);
      checked.unchecked.forEach(c => this.distributionValue[c] = null);
    },
    distributionChanged: function(index) {
      if (this.distributionValue[index] > 100) {
        this.distributionValue[index] = 100;
      }
      let balance = 100 - this.distributionValue[index];
      let checked = this.getCheckedIndex();
      let othersLength = checked.checked.length - 1;

      if (othersLength == 0) {
        this.distributionValue[index] = 100;
      } else {
        let othersValue = balance / othersLength;
        checked.checked.forEach(c => {
          if (c != index) {
            this.distributionValue[c] = othersValue;
          }
        });
      }
    }
  },
  mounted() {
    this.checkPercentage();
  },
  watch: {
    distributionCheck: function() {
      this.checkPercentage();
    }
  }
};
</script>

<style lang="scss" scoped>
.randomizer-main {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
  padding: 5px;
  // background-color: red;

  > .setup-section {
    display: flex;
    height: 100%;
    width: 100%;
    margin-top: 10px;

    > .setup-left, > .setup-right {
      overflow: hidden;
      flex: 1 1 0px;
      min-width: 0;
      display: flex;
      flex-direction: column;

      > .title {
        margin-bottom: 10px;
        font-weight: bold;
      }
    }

    > .setup-right > input {
      max-width: 100px;
    }

    > .setup-left {
      align-items: flex-end;
      text-align: right;

      > .checkbox-section {
        margin-bottom: 5px;

        > input {
          width: 100px;
          max-width: 100px;
          text-align: right;
        }

        > span {
          cursor: pointer;
          min-width: 120px;
          text-align: left;
        }

        > .checkbox {
          display: flex;
          align-items: center;
        }
      }
    }

    > .splitter {
      // height: 100%;
      width: 1px;
      background-color: black;
    }

    > div {
      margin: 0 5px;
    }
  }

  > .delay-section {
    display: flex;
    flex-direction: column;
    align-items: center;

    > input {
      max-width: 100px;
      text-align: right;
      margin-top: 5px;
    }
  }

  > .error {
    color: red;
  }

  > .button-section { 
    display: flex;
  }

  > .random-section {
    display: flex;
    flex-direction: column;
    align-items: center;
    width: 100%;

    > table {
      border-collapse: collapse;
    }

    > table > tr > td:last-child {
      width: 50px;
      text-align: right;
    }

    > table > tr.border-top > td {
      border-top: 1px solid black;
    }

    > .current-size {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 100%;

      > span {
        width: 100px;
        text-align: right;
      }
    }
  }
}

button {
  border-radius: 20%;
  padding: 10px;
  width: 100px;
  margin: 10px;
}
</style>