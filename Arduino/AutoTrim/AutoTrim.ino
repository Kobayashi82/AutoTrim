#define ENA 8
#define DIR 9
#define PUL 10

enum MotorState {
  STOPPED,
  STEP_MOVE,
  CONT_MOVE
};

MotorState status = STOPPED;
bool direction = LOW;
int stepRemaining = 0;
unsigned long pulseLast = 0;
bool pulseState = LOW;
int stepDelay = 150; // microseconds

void setup() {
  pinMode(ENA, OUTPUT);
  pinMode(DIR, OUTPUT);
  pinMode(PUL, OUTPUT);
  
  digitalWrite(ENA, HIGH);  // Disabled
  digitalWrite(DIR, LOW);
  digitalWrite(PUL, LOW);
  
  Serial.begin(9600);
}

void loop() {
  processCommands();
  generatePulses();
}

void processCommands() {
  if (Serial.available() > 0) {
    String command = Serial.readStringUntil('\n');
    command.trim();
    
    if (command == "S") {                                   // Stop
      stopMotor();

    } else if (command.startsWith("D:")) {                  // Trim Down Continuos
      int speed = command.substring(2).toInt();
      if (speed >= 50 && speed <= 5000) stepDelay = speed;
      moveContinuous(LOW);     
    } else if (command.startsWith("DS:")) {                 // Trim Down Steps
      int steps = command.substring(3).toInt();
      int speed = command.substring(3).toInt();
      if (speed >= 50 && speed <= 5000) stepDelay = speed;
      MoveStep(LOW, steps);

    } else if (command.startsWith("U:")) {                  // Trim Up Continuos
      int speed = command.substring(2).toInt();
      if (speed >= 50 && speed <= 5000) stepDelay = speed;
      moveContinuous(HIGH);
    } else if (command.startsWith("US:")) {                 // Trim Up Steps
      int steps = command.substring(3).toInt();
      int speed = command.substring(3).toInt();
      if (speed >= 50 && speed <= 5000) stepDelay = speed;
      MoveStep(HIGH, steps);
     }
  }
}

void stopMotor() {
  status = STOPPED;
  stepRemaining = 0;
  digitalWrite(PUL, LOW);
  digitalWrite(ENA, HIGH);
}

void moveContinuous(bool dir) {
  if (status == STOPPED) digitalWrite(ENA, LOW);
  
  direction = dir;
  digitalWrite(DIR, direction);
  
  status = CONT_MOVE;
  pulseLast = micros();
  pulseState = LOW;
}

void MoveStep(bool dir, int steps) {
  if (status == STOPPED) digitalWrite(ENA, LOW);
  
  direction = dir;
  digitalWrite(DIR, direction);
  
  stepRemaining = steps;
  status = STEP_MOVE;

  pulseLast = micros();
  pulseState = LOW;
}

void generatePulses() {
  if (status == STOPPED) return;
  
  unsigned long pulseCurr = micros();
  if (pulseCurr - pulseLast >= stepDelay) {
    pulseLast = pulseCurr;
    pulseState = !pulseState;
    digitalWrite(PUL, pulseState);
    
    if (!pulseState && status == STEP_MOVE) {
      stepRemaining--;
      if (stepRemaining <= 0) stopMotor();
    }
  }
}
