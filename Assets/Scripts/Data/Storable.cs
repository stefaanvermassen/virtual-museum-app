using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface Storable<T,D> where D: Data<T> {

    D Save();
    void Load(D data);

}
