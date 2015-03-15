using UnityEngine;
using System.Collections;
using System;

public interface Savable<T,D> : SavableData, Storable<T,D> where D: Data<T> {

}
