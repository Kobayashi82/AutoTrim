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
int stepDelay = 30; // microseconds
unsigned long tiempoInicio = 0;

// Variables para rampa de aceleración
int stepDelayTarget = 30;    // Velocidad objetivo
int stepDelayStart = 700;     // Velocidad inicial (lenta)
unsigned long stepCounter = 0;          // Contador de pasos para aceleración

void setup() {
  pinMode(ENA, OUTPUT);
  pinMode(DIR, OUTPUT);
  pinMode(PUL, OUTPUT);
  
  digitalWrite(ENA, LOW);  // Disabled
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
      if (speed >= 30 && speed <= 5000) {
        stepDelayTarget = speed;
      }
      moveContinuous(LOW);     
    } else if (command.startsWith("DS:")) {                 // Trim Down Steps
      int firstColon = command.indexOf(':', 3);
  
      int steps = command.substring(3, firstColon).toInt();
      int speed = command.substring(firstColon + 1).toInt();
      if (speed >= 30 && speed <= 5000) stepDelayTarget = speed;
      MoveStep(LOW, steps);

    } else if (command.startsWith("U:")) {                  // Trim Up Continuos
      int speed = command.substring(2).toInt();
      if (speed >= 30 && speed <= 5000) {
        stepDelayTarget = speed;
      }
      moveContinuous(HIGH);
    } else if (command.startsWith("US:")) {                 // Trim Up Steps
      int firstColon = command.indexOf(':', 3);
  
      int steps = command.substring(3, firstColon).toInt();
      int speed = command.substring(firstColon + 1).toInt();
      if (speed >= 30 && speed <= 5000) stepDelayTarget = speed;
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
  // digitalWrite(ENA, HIGH);
  delayMicroseconds(10000);
}

void moveContinuous(bool dir) {
  if (direction != dir) stopMotor();
  
  if (status == STOPPED) {
    // digitalWrite(ENA, LOW);
    tiempoInicio = millis();
  }
  
  if (direction != dir) {          // cambio de dirección
    direction = dir;
    digitalWrite(DIR, direction);
    delayMicroseconds(15000);        // pausa de estabilización
  }
  
  stepRemaining = 0;
  status = CONT_MOVE;
  
  // Reinicia aceleración también para continuo
  stepDelay = stepDelayStart;
  stepCounter = 0;
  
  pulseLast = micros();
  pulseState = LOW;
}

void MoveStep(bool dir, int steps) {
  if (direction != dir) stopMotor();
  
  if (status == STOPPED) {
    // digitalWrite(ENA, LOW);
    tiempoInicio = millis();
  }
  
  if (direction != dir) {          // cambio de dirección
    direction = dir;
    digitalWrite(DIR, direction);
    delayMicroseconds(15000);        // pausa de estabilización
  }
  
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

      // Aceleración progresiva suave (curva exponencial)
      if (stepDelay > stepDelayTarget) {
        float progress = (float)stepCounter / 300.0; // controla rapidez; menor = acelera antes
        if (progress > 1.0) progress = 1.0;
        float accelFactor = 1 - exp(-4 * progress);  // sube rápido, se suaviza al final
        int newDelay = stepDelayStart - (stepDelayStart - stepDelayTarget) * accelFactor;
        stepDelay = newDelay;
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