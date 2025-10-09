#define ENA 8
#define DIR 9
#define PUL 10

enum MotorState {
  STOPPED,
  STEP_MOVE,
  CONT_MOVE
};

MotorState    status = STOPPED;
bool          direction = LOW;
int           stepDelay = 30; // microseconds
int           stepRemaining = 0;
bool          pulseState = LOW;
unsigned long pulseLast = 0;

int           stepDelayTarget = 30;
int           stepDelayStart = 700;
unsigned long stepCounter = 0;

void setup() {
  pinMode(ENA, OUTPUT);
  pinMode(DIR, OUTPUT);
  pinMode(PUL, OUTPUT);

  Serial.begin(9600);
}

void loop() {
  if (Serial.available()) {
    String command = Serial.readStringUntil('\n');
    command.trim();
    
    // Stop
    if (command == "S") {
      stopMotor();

    // Trim Up
    } else if (command.startsWith("US:")) {
        int firstColon = command.indexOf(':', 3);
      stepDelayTarget = max(30, min(700, command.substring(firstColon + 1).toInt()));
      int steps = max(0, min(20000, command.substring(3, firstColon).toInt()));
      moveMotor(!steps ? CONT_MOVE : STEP_MOVE, HIGH, steps);

    // Trim Down
    } else if (command.startsWith("DS:")) {
      int firstColon = command.indexOf(':', 3);
      stepDelayTarget = max(30, min(700, command.substring(firstColon + 1).toInt()));
      int steps = max(0, min(20000, command.substring(3, firstColon).toInt()));
      moveMotor(!steps ? CONT_MOVE : STEP_MOVE, LOW, steps);
    }
  }

  if (status != STOPPED) { 
    unsigned long pulseCurr = micros();
    if (pulseCurr - pulseLast >= stepDelay) {
      pulseLast = pulseCurr;
      pulseState = !pulseState;
      digitalWrite(PUL, pulseState);
      
      if (!pulseState) {
        stepCounter++;

        if (stepDelay > stepDelayTarget) {
          float accelFactor = 1 - exp(-4 * min((float)stepCounter / 1500.0, 1.0));
          stepDelay = stepDelayStart - (stepDelayStart - stepDelayTarget) * accelFactor;;
          if (stepDelay <= stepDelayTarget) stepDelay = stepDelayTarget;
        }

        if (status == STEP_MOVE && --stepRemaining <= 0) stopMotor();
      }
    }
  }
}

void stopMotor() {
  status = STOPPED;
  stepRemaining = 0;
  delayMicroseconds(10000);
  pulseState = LOW;
  digitalWrite(PUL, pulseState);
}

void moveMotor(int mode, bool dir, int steps) {
  if (direction != dir) {
    direction = dir;
    stopMotor();
    digitalWrite(DIR, direction);
  }

  delayMicroseconds(10000);
  status = mode;
  stepRemaining = steps;
  stepDelay = stepDelayStart;
  stepCounter = 0;
  pulseLast = micros();
}
