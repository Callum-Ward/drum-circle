int threshold = 2;
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

//Used to test signal waveform produced from Pizeos and FSRs at the start of the project

void loop() {
  // put your main code here, to run repeatedly:
  int val1 = analogRead(0);
  int val2 = analogRead(1);
  int val3 = analogRead(2);  
  int val4 = analogRead(3);  
  
  if (val1 > threshold) {
    Serial.print("Serial 0: ");
    Serial.println(val1);
  }
  if (val2 > threshold) {
    Serial.print("Serial 1: ");
    Serial.println(val2);
  }
  if (val3 > threshold) {
    Serial.print("Serial 2: ");
    Serial.println(val3);
  }
    if (val4 > threshold) {
      Serial.print("Serial 3: ");
    Serial.println(val4);
  }

}
