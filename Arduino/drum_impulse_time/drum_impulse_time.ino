#include <millisDelay.h>;
int threshold = 500;
bool hit = false;
unsigned long delayStart = 0;
int maxVal =0;
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}
void loop() {
  // put your main code here, to run repeatedly:
  int val1 = analogRead(A0);
  int val2 = analogRead(A1);
  int val3 = analogRead(A2);
  if (val1 > maxVal) {
    maxVal = val1;
  }
  if (val1 > threshold && hit==false) {
    hit = true;
    delayStart = millis();
    //Serial.println(val1);
  } else if (hit && val1 < threshold) {
    Serial.print("max: ");
    Serial.println(maxVal);
    Serial.print("impulse time(ms):");
    Serial.println(millis()-delayStart);
    maxVal =0; 
    hit = false;
  }
}
