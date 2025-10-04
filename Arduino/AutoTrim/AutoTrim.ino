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
int stepDelay = 100; // microseconds
unsigned long tiempoInicio = 0;

// Variables para rampa de aceleración
int stepDelayTarget = 300;    // Velocidad objetivo
int stepDelayStart = 700;     // Velocidad inicial (lenta)
int accelRate = 5;            // Cuánto acelerar cada ciertos pasos
int accelSteps = 5;           // Cada cuántos pasos acelerar
int stepCounter = 0;          // Contador de pasos para aceleración

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
      if (speed >= 100 && speed <= 5000) {
        stepDelayTarget = speed;
      }
      moveContinuous(LOW);     
    } else if (command.startsWith("DS:")) {                 // Trim Down Steps
      int firstColon = command.indexOf(':', 3);
  
      int steps = command.substring(3, firstColon).toInt();
      int speed = command.substring(firstColon + 1).toInt();
      if (speed >= 100 && speed <= 5000) stepDelayTarget = speed;
      MoveStep(LOW, steps);

    } else if (command.startsWith("U:")) {                  // Trim Up Continuos
      int speed = command.substring(2).toInt();
      if (speed >= 100 && speed <= 5000) {
        stepDelayTarget = speed;
      }
      moveContinuous(HIGH);
    } else if (command.startsWith("US:")) {                 // Trim Up Steps
      int firstColon = command.indexOf(':', 3);
  
      int steps = command.substring(3, firstColon).toInt();
      int speed = command.substring(firstColon + 1).toInt();
      if (speed >= 100 && speed <= 5000) stepDelayTarget = speed;
      MoveStep(HIGH, steps);
     }
  }
}

void stopMotor() {
  if (status != STOPPED) {  // Solo si estaba en movimiento
    unsigned long tiempoTranscurrido = millis() - tiempoInicio;
    
    Serial.print("Tiempo de movimiento: ");
    Serial.print(tiempoTranscurrido);
    Serial.println(" ms");
  }

  status = STOPPED;
  stepRemaining = 0;
  digitalWrite(PUL, LOW);
  digitalWrite(ENA, HIGH);
}

void moveContinuous(bool dir) {
  if (status == STOPPED) {
    digitalWrite(ENA, LOW);
    tiempoInicio = millis();
  }
  
  direction = dir;
  digitalWrite(DIR, direction);
  
  status = CONT_MOVE;
  
  // Reinicia aceleración también para continuo
  stepDelay = stepDelayStart;
  stepCounter = 0;
  
  pulseLast = micros();
  pulseState = LOW;
}

void MoveStep(bool dir, int steps) {
  if (status == STOPPED) {
    digitalWrite(ENA, LOW);
    tiempoInicio = millis();
  }
  
  direction = dir;
  digitalWrite(DIR, direction);
  
  stepRemaining = steps;
  status = STEP_MOVE;
  
  // Reinicia aceleración
  stepDelay = stepDelayStart;
  stepCounter = 0;

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
    
    if (!pulseState) {
      stepCounter++;
      
      // Aceleración progresiva para ambos modos
      if (stepCounter % accelSteps == 0 && stepDelay > stepDelayTarget) {
        stepDelay -= accelRate;
        if (stepDelay < stepDelayTarget) stepDelay = stepDelayTarget;
      }
      
      // Solo para movimiento por pasos
      if (status == STEP_MOVE) {
        stepRemaining--;
        if (stepRemaining <= 0) stopMotor();
      }
    }
  }
}