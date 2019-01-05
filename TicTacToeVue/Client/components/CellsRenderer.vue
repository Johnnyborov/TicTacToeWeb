<script>
import Resources from '../resources/resources.js'

export default {
  data() {
    return {
      cross: null,
      nought: null,
      clear: null
    }
  },

  render() {
    if (!this.$parent.context) return
    var ctx = this.$parent.context
    
    ctx.lineWidth = 10

    for (let i = 0; i < 3; i++) {
      for (let j = 0; j < 3; j++) {
        ctx.strokeRect(5+130*j, 5+130*i, 110, 110)         

        let cell;
        switch(this.$store.state.gameEngine.cells[i*3+j]) {
          case 0:
            cell = this.$parent.clear
            break
          case 1:
            cell = this.$parent.cross
            break
          case 2:
            cell = this.$parent.nought
            break
          default:
            throw "Invalid cell value"
        }

        ctx.drawImage(cell, 10+130*j, 10+130*i) 
      }    
    }
  }
}
</script>