<template>
  <div class="randomizer-main">
    <div class="title">This page will let you generate a randomize report</div>
    <div class="setup-section">
      <div class="setup-left">
        <div class="title">Set the data distribution</div>
        <div class="checkbox-section">
          <div>Alphanumeric</div>
          <input type="number" v-model.number="alphaDist" :disabled="randomObjects.length > 0">
        </div>
        <div class="checkbox-section">
          <div>Numeric</div>
          <input type="number" v-model.number="numericDist" :disabled="randomObjects.length > 0">
        </div>
        <div class="checkbox-section">
          <div>Float</div>
          <input type="number" v-model.number="floatDist" :disabled="randomObjects.length > 0">
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
    <div class="button-section">
      <button @click="startClicked" :disabled="isRunning">{{ randomObjects.length > 0 ? 'Reset' : 'Start' }}</button>
      <button @click="stopClicked" :disabled="!isRunning">Stop</button>
    </div>

    <div v-if="isRunning || randomObjects.length > 0" class="random-section">
      <table>
        <tr>
          <td>Alphanumeric</td>
          <td>:</td>
          <td>{{ alphaCounter }}</td>
        </tr>
        <tr>
          <td>Integer</td>
          <td>:</td>
          <td>{{ integerCounter }}</td>
        </tr>
        <tr>
          <td>Float</td>
          <td>:</td>
          <td>{{ floatCounter }}</td>
        </tr>
        <tr class="border-top">
          <td>Total</td>
          <td>:</td>
          <td>{{ floatCounter + integerCounter + alphaCounter }}</td>
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
      checkData: [],
      outputSize: '',
      
      randomObjects: [],
      alphaCounter: 0,
      integerCounter: 0,
      floatCounter: 0,

      alphaDist: '',
      numericDist: '',
      floatDist: '',

      delay: 0,

      isRunning: false
    }
  },
  props: {
  },
  methods: {
    startClicked: async function() {
      if (this.randomObjects.length == 0) {
        this.isRunning = true;
        while (this.isRunning) {
          await this.getRandom();
          await this.$sleep(this.delay);
        }
      } else {
        this.randomObjects = [];
        this.alphaCounter = 0;
        this.integerCounter = 0;
        this.floatCounter = 0;
      }
    },
    stopClicked: function() {
      this.isRunning = false;
    },
    getRandom: async function() {
      var random = await this.$axios.get(`/apiv1/GetRandom?distribution=${this.getDistribution()}`);
      switch (random.data['type']) {
        case 'Alphanumeric':
          this.alphaCounter++;
          break;
        case 'Integer':
          this.integerCounter++;
          break;
        case 'Float':
          this.floatCounter++;
          break;
      }
      this.randomObjects.push(random.data['value']);
      if (this.outputSize > 0 && this.randomObjects.join(',').length / 1000 >= this.outputSize) {
        this.stopClicked();
      }
    },
    getDistribution: function() {
      if (!this.alphaDist || !this.numericDist || !this.floatDist) {
        return [0,1,2].join(',');
      }

      var dist = [];
      if ((this.alphaCounter / this.randomObjects.length * 100) <= this.alphaDist) {
        dist.push(0);
      }
      if ((this.integerCounter / this.randomObjects.length * 100) <= this.numericDist) {
        dist.push(1);
      }
      if ((this.floatCounter / this.randomObjects.length * 100) <= this.floatDist) {
        dist.push(2);
      }

      return dist.join(',');
    },
    generateReport: async function() {
      var generate = await this.$axios.post('/apiv1/GenerateReport', this.randomObjects.join(','));
      if (generate.data) {
        this.$store.state.canLoadReport = true;
        this.$router.push('/Report');
      }
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
          max-width: 100px;
          text-align: right;
        }

        > span {
          cursor: pointer;
          min-width: 120px;
          text-align: left;
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